using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Windows.Forms;

using System.Runtime.InteropServices;

using vJoyInterfaceWrap;

namespace PP2vJoyFeeder {
  /*
   * ToDo - implement handling of AxisRX, AxisRZ, ContPov's, DiscPov's and analog joysticks
   *  Currently only digital joysticks are supported
   *  The only thing this has really been tested with is an "atari 2600" style joystick.
   *    i.e. four direction joystick with single "fire" button.
   *  This was really developed to provide a way for a parallel port attached atari joystick
   *   to be useable by C64 emulators.  The same joystick hardware was already natively recognized
   *   by Amiga emulator but was not recognized by C64 emulators.  So this is the answer.
   *  The code includes handling a third "Z" axis but there was no way to test this since such a
   *   joystick was unavailable.  If someone wants to test a "Z" axis joystick or implement any
   *   of the missing functionality mentioned above they are more than welcome.  This project
   *   will likely not see any updates since it now meets the original needs.
   *  This project depends on two other projects and is basically glue to wire those other two
   *   projects together.  It requires InpOut32 (x64) which can be found here:
   *     http://highrez.co.uk/downloads/inpout32/default.htm
   *   and it requries vJoy which can be found here:
   *     http://vjoystick.sourceforge.net/site/
  */
  static class Program {
    // import inpout functions
    [DllImport("inpout32.dll")]
    private static extern UInt32 IsInpOutDriverOpen ();
    [DllImport("inpout32.dll")]
    private static extern void Out32 (short PortAddress, short Data);
    [DllImport("inpout32.dll")]
    private static extern char Inp32 (short PortAddress);

    [DllImport("inpout32.dll")]
    private static extern void DlPortWritePortUshort (short PortAddress, ushort Data);
    [DllImport("inpout32.dll")]
    private static extern ushort DlPortReadPortUshort (short PortAddress);

    [DllImport("inpout32.dll")]
    private static extern void DlPortWritePortUlong (int PortAddress, uint Data);
    [DllImport("inpout32.dll")]
    private static extern uint DlPortReadPortUlong (int PortAddress);

    // now the 64bit versions
    [DllImport("inpoutx64.dll")]
    private static extern bool GetPhysLong (ref int PortAddress, ref uint Data);
    [DllImport("inpoutx64.dll")]
    private static extern bool SetPhysLong (ref int PortAddress, ref uint Data);

    [DllImport("inpoutx64.dll", EntryPoint = "IsInpOutDriverOpen")]
    private static extern UInt32 IsInpOutDriverOpen_x64 ();
    [DllImport("inpoutx64.dll", EntryPoint = "Out32")]
    private static extern void Out32_x64 (short PortAddress, short Data);
    [DllImport("inpoutx64.dll", EntryPoint = "Inp32")]
    private static extern char Inp32_x64 (short PortAddress);

    [DllImport("inpoutx64.dll", EntryPoint = "DlPortWritePortUshort")]
    private static extern void DlPortWritePortUshort_x64 (short PortAddress, ushort Data);
    [DllImport("inpoutx64.dll", EntryPoint = "DlPortReadPortUshort")]
    private static extern ushort DlPortReadPortUshort_x64 (short PortAddress);

    [DllImport("inpoutx64.dll", EntryPoint = "DlPortWritePortUlong")]
    private static extern void DlPortWritePortUlong_x64 (int PortAddress, uint Data);
    [DllImport("inpoutx64.dll", EntryPoint = "DlPortReadPortUlong")]
    private static extern uint DlPortReadPortUlong_x64 (int PortAddress);

    [DllImport("inpoutx64.dll", EntryPoint = "GetPhysLong")]
    private static extern bool GetPhysLong_x64 (ref int PortAddress, ref uint Data);
    [DllImport("inpoutx64.dll", EntryPoint = "SetPhysLong")]
    private static extern bool SetPhysLong_x64 (ref int PortAddress, ref uint Data);

    static bool m_bX64 = false;

    static Dictionary<string, Joystick> config = new Dictionary<string, Joystick>();

    static Joystick currentJoystick;

    static FormMain fm;

    static vJoy vJoystick;

    [STAThread]
    static void Main () {
      readConfig();
      vJoystick = new vJoy();
      fm = new FormMain();
      getVJoyList();

      // set up the output textbox stuff
      int lines = fm.txtOutput.ClientSize.Height / fm.txtOutput.Font.Height;
      string[] output = new string[lines];
      for (int i = 0; i < lines; i++) {
        output[i] = "";
      }
      int count = 0;

      // set up the joystick list box and the vJoy device list box
      foreach (KeyValuePair<string, Joystick> pair in config) {
        fm.cbJoyType.Items.Add(pair.Key);
      }
      fm.cbJoyType.DropDownStyle = ComboBoxStyle.DropDownList;
      fm.cbJoyType.SelectedIndex = 0;
      fm.cbVJoyDev.DropDownStyle = ComboBoxStyle.DropDownList;
      fm.cbVJoyDev.SelectedIndex = 0;
      joystickSelected();
      if (currentJoystick == null) {
        MessageBox.Show("No joystick selected");
        Application.Exit();
      }
      fm.Show();
      if (!initPort()) {
        MessageBox.Show("Could not open parallel port:" + currentJoystick.getPort());
        fm.Close();
        Application.Exit();
      }

      // set up some variables for the main loop
      bool changed = true;
      long maxval = 0;
      long minval = 10;
      int curX, curY, curZ;
      vJoy.JoystickState iReport = new vJoy.JoystickState();
      vJoystick.GetVJDAxisMax(currentJoystick.getId(), HID_USAGES.HID_USAGE_X, ref maxval);
      Console.WriteLine("\nAxis max range value = " + maxval);
      vJoystick.GetVJDAxisMin(currentJoystick.getId(), HID_USAGES.HID_USAGE_X, ref minval);
      Console.WriteLine("\nAxis min range value = " + minval);
      vJoystick.ResetVJD(currentJoystick.getId());
    
      // main loop
      while (fm.Created) {
        Application.DoEvents();
        long l = readLongFromPort(currentJoystick.getPort());
        curX = (int)(maxval - minval) / 2;
        curY = (int)(maxval - minval) / 2;
        curZ = (int)(maxval - minval) / 2;
        iReport.Buttons = 0;
        iReport.bDevice = (byte)currentJoystick.getId(); // inside the main loop in case the vJoy device changes
        // first check if the joystick is in "neutral" i.e. no buttons or stick movements active.
        if ((l & currentJoystick.getPPortInfo()["none"][0]) == currentJoystick.getPPortInfo()["none"][1]) {
          if (changed) {
            changed = false;
            vJoystick.ResetVJD(currentJoystick.getId());
            if (fm.ckOutputOn.Checked) {
              output[count % lines] = "" + count + " : none:::" + l + ":" + 
                                      currentJoystick.getPPortInfo()["none"][0] + ":" +
                                      currentJoystick.getPPortInfo()["none"][1] + "\r\n";
              fm.txtOutput.Text = "";
              for (int j = 0; j < lines; j++) {
                fm.txtOutput.Text += output[((count + j + 1) % lines)];
              }
              count++;
            }
          }
        } else {
          // we must be getting something from the joystick so process it accordingly
          foreach (KeyValuePair<string, string> pair in currentJoystick.getVJoyInfo()) {
            int[] mv;
            // loop through the vJoy entries and look for state matches
            if (currentJoystick.getPPortInfo().TryGetValue(pair.Value, out mv)) {
              if ((l & mv[0]) == mv[1]) {
                if (pair.Key.Contains("-min")) {
                  if (pair.Key.Contains("AxisX")) {
                    curX = (int)minval;
                  }
                  if (pair.Key.Contains("AxisY")) {
                    curY = (int)minval;
                  }
                  if (pair.Key.Contains("AxisZ")) {
                    curZ = (int)minval;
                  }
                }
                if (pair.Key.Contains("-max")) {
                  if (pair.Key.Contains("AxisX")) {
                    curX = (int)maxval;
                  }
                  if (pair.Key.Contains("AxisY")) {
                    curY = (int)maxval;
                  }
                  if (pair.Key.Contains("AxisZ")) {
                    curZ = (int)maxval;
                  }
                }
                if (pair.Key.Contains("Button")) {
                  // process the Buttons
                  int b = int.Parse(pair.Key.Substring(6));
                  iReport.Buttons = iReport.Buttons | (uint)(2 ^ b);
                }
                changed = true;
                iReport.AxisX = curX;
                iReport.AxisY = curY;
                iReport.AxisZ = curZ;
                vJoystick.UpdateVJD(currentJoystick.getId(), ref iReport);
                if (fm.ckOutputOn.Checked) {
                  // update the logging text box (useful for determining needed values)
                  output[count % lines] = "" + count + " : " + pair.Key + ":" + pair.Value +
                                          ":" + l + ":" + mv[0] + ":" + mv[1] + "\r\n";
                  fm.txtOutput.Text = "";
                  for (int j = 0; j < lines; j++) {
                    fm.txtOutput.Text += output[((count + j + 1) % lines)];
                  }
                  count++;
                }
              }
            }
          }
        }
        System.Threading.Thread.Sleep(currentJoystick.getPollingRate());
      }
    }

    static void getVJoyList () {
      if ((vJoystick == null) || !vJoystick.vJoyEnabled()) {
        MessageBox.Show("vJoy driver not enabled: Failed Getting vJoy attributes.\n");
        Application.Exit();
      }
      int found = 0;
      for (uint id = 1; id < 17; id++) {
        VjdStat status = vJoystick.GetVJDStatus(id);
        switch (status) {
          case VjdStat.VJD_STAT_OWN:
            found++;
            fm.cbVJoyDev.Items.Add(id);
            break;
          case VjdStat.VJD_STAT_FREE:
            found++;
            fm.cbVJoyDev.Items.Add(id);
            break;
          case VjdStat.VJD_STAT_BUSY:
            MessageBox.Show("vJoy Device "+id+" is already owned by another feeder.");
            break;
          case VjdStat.VJD_STAT_MISS:
            break;
          default:
            MessageBox.Show("vJoy Device "+id+" general error.\nCannot continue.");
            Application.Exit();
            break;
        }
      }
      if (found == 0) {
        MessageBox.Show("No defined vJoy devices found.");
        Application.Exit();
      }
    }

    static void readConfig () {
      XmlTextReader reader = new XmlTextReader("PP2vJoyFeederConfig.xml");
      Joystick tempJS;
      reader.WhitespaceHandling = WhitespaceHandling.None;
      while (reader.Read()) {
        if (reader.NodeType == XmlNodeType.Element) {
          if (reader.LocalName.Equals("joystick")) {
            // we are in a joystick definition which includes a set of Parallel Port definitions
            //  and a set of vJoy definitions.  These sets go together to form a complete joystick
            //  definition which is stored in the Dictionary as a Joystick using the name as the key.
            tempJS = new Joystick();
            reader.MoveToContent();
            nextElement(reader, "name");
            tempJS.setName(reader.ReadElementContentAsString());
            nextElement(reader, "pollingRate");
            tempJS.setPollingRate(reader.ReadElementContentAsInt());
            nextElement(reader, "PPortInfo");
            reader.MoveToContent();
            nextElement(reader, "portNum");
            tempJS.setPort(reader.ReadElementContentAsInt());
            nextElement(reader, "type");
            tempJS.setType(reader.ReadElementContentAsString());
            nextElement(reader, "values");
            string tKey;
            while (reader.Read() && !(reader.NodeType == XmlNodeType.EndElement && reader.LocalName.Equals("values"))) {
              // loop to read all the joystick state value pairs
              reader.MoveToContent();
              tKey = reader.LocalName;
              int[] tMV = new int[2];
              nextElement(reader, "mask");
              tMV[0] = reader.ReadElementContentAsInt();
              nextElement(reader, "value");
              tMV[1] = reader.ReadElementContentAsInt();
              tempJS.addPPortInfoEntry(tKey, tMV);
            }
            nextElement(reader, "VJoyInfo");
            reader.Read();
            while (!(reader.NodeType == XmlNodeType.EndElement && reader.LocalName.Equals("VJoyInfo"))) {
              // loop to read all the joystick state value pairs
              reader.MoveToContent();
              tempJS.addVJoyInfoEntry(reader.LocalName, reader.ReadElementContentAsString());
            }
            config.Add(tempJS.getName(), tempJS);
          }
        }
      }
    }

    static void nextElement (XmlTextReader reader, string element) {
      if ((reader.NodeType == XmlNodeType.Element) && (reader.LocalName.Equals(element))) {
        return;
      }
      while (reader.Read() && (reader.NodeType != XmlNodeType.Element)) {
        // get to the next element
      }
      if (!reader.LocalName.Equals(element)) {
        MessageBox.Show("Error reading joystick configuration looking for <"+element+">");
        Application.Exit();
      }
    }

    public static void joystickSelected () {
      if (!config.TryGetValue(fm.cbJoyType.SelectedItem.ToString(), out currentJoystick)) {
        currentJoystick = config.ElementAt(0).Value;
      }
      // now we need to make sure the vJoy device matches defined capabilities of PPort joystick
      if (fm.cbVJoyDev.SelectedIndex == -1) {
        return;
      }
      uint id = (uint)fm.cbVJoyDev.SelectedIndex + 1;
      int nButtons = vJoystick.GetVJDButtonNumber(id);
      int ContPovNumber = vJoystick.GetVJDContPovNumber(id);
      int DiscPovNumber = vJoystick.GetVJDDiscPovNumber(id);
      foreach (KeyValuePair<string, string> pair in currentJoystick.getVJoyInfo()) {
        if (pair.Key.Contains("Button")) {
          // handle Buttons
          int b = int.Parse(pair.Key.Substring(6));
          if (b > nButtons) {
            MessageBox.Show("This vJoy device supports "+nButtons+" buttons.\nThis joystick defines button #"+b+".\nSelect another vJoy device.");
            return;
          }
        }
        if (pair.Key.Contains("AxisX")) {
          // AxisX check
          if (!vJoystick.GetVJDAxisExist(id, HID_USAGES.HID_USAGE_X)) {
            MessageBox.Show("Currently selected vJoy device does not support Axis X.\nSelect another vJoy device");
            return;
          }
        }
        if (pair.Key.Contains("AxisY")) {
          // AxisY check
          if (!vJoystick.GetVJDAxisExist(id, HID_USAGES.HID_USAGE_Y)) {
            MessageBox.Show("Currently selected vJoy device does not support Axis Y.\nSelect another vJoy device");
            return;
          }
        }
        if (pair.Key.Contains("AxisZ")) {
          // AxisZ check
          if (!vJoystick.GetVJDAxisExist(id, HID_USAGES.HID_USAGE_Z)) {
            MessageBox.Show("Currently selected vJoy device does not support Axis Z.\nSelect another vJoy device");
            return;
          }
        }
        // todo - check AxisRX, AxisRZ, ContPov's, and DiscPov's
      }
      VjdStat status = vJoystick.GetVJDStatus(id);
      if (status != VjdStat.VJD_STAT_OWN) {
        if ((status == VjdStat.VJD_STAT_FREE) && (!vJoystick.AcquireVJD(id))) {
          MessageBox.Show("Failed to acquire vJoy device number " + id + ".");
          return;
        }
      }
      currentJoystick.setId(id);
    }

    static long readLongFromPort (int port) {
      uint l = 0;

      try {
        if (m_bX64) {
          l = DlPortReadPortUlong_x64(port);
        } else {
          l = DlPortReadPortUlong(port);
        }
      } catch (Exception ex) {
        MessageBox.Show("An error occured reading port:\n" + ex.Message);
      }
      return l;
    }

    static bool initPort () {
      try {
        uint nResult = 0;
        try {
          nResult = IsInpOutDriverOpen();
        } catch (BadImageFormatException) {
          nResult = IsInpOutDriverOpen_x64();
          if (nResult != 0) {
            m_bX64 = true;
          }
        }

        if (nResult == 0) {
          MessageBox.Show("Unable to open InpOut32 driver");
          return false;
        }
      } catch (DllNotFoundException ex) {
        MessageBox.Show("Unable to find InpOut32.dll\n" + ex.ToString());
        return false;
      }
      return true;
    }
  }
}
