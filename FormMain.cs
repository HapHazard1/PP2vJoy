using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PP2vJoyFeeder {
  public partial class FormMain : Form {
    public FormMain() {
      InitializeComponent();
    }

    private void cbJoyType_SelectedIndexChanged (object sender, EventArgs e) {
      Program.joystickSelected();
    }

    private void cbVJoyDev_SelectedIndexChanged (object sender, EventArgs e) {
      Program.joystickSelected();
    }
  }
}
