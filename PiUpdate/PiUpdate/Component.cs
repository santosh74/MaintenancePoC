using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

public class Component
{
    private string name;
    private string GPIO;
    private string currentCharge;
    public string previousCharge;



    //Default constructor
    public Component(string name, string GPIO)
    {
        this.name = name;
        this.GPIO = GPIO;
        if (!Directory.Exists("/sys/class/gpio/" + this.GPIO + "/"))
        {
            this.Export();
        }
        this.update();
    }

    public string getGPIO()
    {
        return this.GPIO;
    }
    public string getName()
    {
        return this.name;
    }

    //Setters and getters
    public void setCurrentCharge(string currentCharge)
    {
        this.currentCharge = currentCharge;
    }

    public void setPreviousCharge(string previousCharge)
    {
        this.previousCharge = previousCharge;
    }

    //Methods

    //Method for checked whether a components state has changed or not
    public bool isChanged()
    {
        if (this.currentCharge != this.previousCharge)
        {
            return true;
        }
        return false;
    }

    //Creates an object specific JSON string
    public string getJson()
    {
        return "{\"name\": \"" + this.name + "\", \"state\": " + this.currentCharge + "}";
    }

    //Returns the Pin specific number for exporting
    public string getPinNumber()
    {

        return this.GPIO.Remove(0, 4);
    }

    //Updates the current state of the switch
    public void update()
    {
        this.previousCharge = this.currentCharge;
        this.currentCharge = Regex.Replace(File.ReadAllText("/sys/class/gpio/" + this.GPIO + "/value"), @"\t|\n|\r", "");
        if (this.name.Contains("DOOR") || this.name.Contains("KEY"))
        {
            if (this.currentCharge == "0")
            {
                this.currentCharge = "1";
            }
            else if (this.currentCharge == "1")
            {
                this.currentCharge = "0";
            }
        }

    }

    public void Export()
    {
        //Console.WriteLine(component.getPinNumber());
        File.WriteAllText("/sys/class/gpio/export", this.getPinNumber());
        File.WriteAllText("/sys/class/gpio/" + this.getGPIO() + "/direction", "in");
    }
}

//Electric box class
public class ElectricBox
{
    private List<Component> components;

    //Default constructor
    public ElectricBox()
    {
        components = new List<Component>();
    }

    //Parameterized constructor
    public ElectricBox(List<Component> components)
    {
        this.components = components;
    }

    //Methods

    //Adds a component to the component list
    public void Add(Component component)
    {
        this.components.Add(component);
    }

    //Creates a json list for all the elements existing within the box and their current states
    public string getCurrentState()
    {
        string currentStateJson = "[";
        foreach (Component component in components)
        {
            if (currentStateJson.Length > 1)
            {
                currentStateJson += ", " + component.getJson();
            }
            else if (currentStateJson.Length == 1)
            {
                currentStateJson += component.getJson();
            }
        }
        currentStateJson += "]";
        return currentStateJson;
    }

    //Getter for the component list
    public List<Component> getComponents()
    {
        return this.components;
    }
}