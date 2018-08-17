using System.Collections;
using System.Collections.Generic;


public class VertxAnimation
{
    public string name { get; set; }
    public int state { get; set; }
    public string id { get; set; }
    public bool done { get; set; }

    public VertxAnimation(string name, string id, int state, bool done)
    {
        this.name = name;
        this.id = id;
        this.state = state;
        this.done = done;
    }
}