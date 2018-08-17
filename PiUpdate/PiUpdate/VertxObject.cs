using System.Collections.Generic;

public class Transform
{
    public List<float> position { get; set; }
    public List<float> rotation { get; set; }
    public List<float> scale { get; set; }
}

public class Transform2
{
    public List<float> position { get; set; }
    public List<float> rotation { get; set; }
    public List<float> scale { get; set; }
}

public class Components
{
}

public class Child
{
    public string id { get; set; }
    public object prefab { get; set; }
    public object tag { get; set; }
    public string guid { get; set; }
    public int layers { get; set; }
    public object assets { get; set; }
    public Transform2 transform { get; set; }
    public List<object> children { get; set; }
    public Components components { get; set; }
}

public class Components2
{
}

public class RootNode
{
    public object id { get; set; }
    public object prefab { get; set; }
    public object tag { get; set; }
    public string guid { get; set; }
    public int layers { get; set; }
    public object assets { get; set; }
    public Transform transform { get; set; }
    public List<Child> children { get; set; }
    public Components2 components { get; set; }
}

public class Assets
{
}

public class VertxObject
{
    public string id { get; set; }
    public string friendlyName { get; set; }
    public int layerMask { get; set; }
    public RootNode rootNode { get; set; }
    public Assets assets { get; set; }
}