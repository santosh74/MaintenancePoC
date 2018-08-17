using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WindowText
{
    public string WindowName { get; set; }
    public string WindowTitle { get; set; }
    public string WindowDescription { get; set; }
}

public class ResponseObject {

    public List<WindowText> WindowText { get; set; }
}
