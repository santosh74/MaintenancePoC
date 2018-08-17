using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

public class Component
{
    private string name;
    private string currentCharge = "0";
    public string previousCharge = "0";



    //Default constructor
    public Component(string name)
    {
        this.name = name;
        if ((name == "KEY_ANIMATION" || name == "DOOR_ANIMATION" || name == "SWITCH_TWO"))
        {
            this.currentCharge = "0";
            this.previousCharge = this.currentCharge;
        }
        else
        {
            this.currentCharge = "1";
            this.previousCharge = this.currentCharge;
        }
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

    //Updates the current state of the switch
    public void UpdateFromKeyboard(bool changeValue)
    {
        this.previousCharge = this.currentCharge;

        if (changeValue)
        {
            if (this.currentCharge == "0")
                this.currentCharge = "1";
            else
                this.currentCharge = "0";
        }
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