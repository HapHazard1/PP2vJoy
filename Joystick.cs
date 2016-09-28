using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PP2vJoyFeeder {
  class Joystick {
    string name = "";
    int pollingRate = 0;
    string type = "";
    int port = 888;
    uint id = 1;
    Dictionary<string, int[]> pportInfo = new Dictionary<string,int[]>();
    Dictionary<string, string> vJoyInfo = new Dictionary<string,string>();

    public void setName (string n) {
      name = n;
    }
    public string getName () {
      return name;
    }

    public void setPollingRate (int p) {
      pollingRate = p;
    }
    public int getPollingRate () {
      return pollingRate;
    }

    public void setType (string t) {
      type = t;
    }
    public string getType () {
      return type;
    }

    public void setPort (int p) {
      port = p;
    }
    public int getPort () {
      return port;
    }

    public void setId (uint i) {
      id = i;
    }
    public uint getId () {
      return id;
    }

    public void addPPortInfoEntry (string k, int[] v) {
      pportInfo.Add(k, v);
    }
    public Dictionary<string, int[]> getPPortInfo () {
      return pportInfo;
    }

    public void addVJoyInfoEntry (string k, string v) {
      vJoyInfo.Add(k, v);
    }
    public Dictionary<string, string> getVJoyInfo () {
      return vJoyInfo;
    }
  }
}
