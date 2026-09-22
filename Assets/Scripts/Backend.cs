// Backend.cs — backend of term system

// Periodically save running data
// Save IO data immediately when prompted
// Seperate "environment" variables for each term/server whatever
// memory/processing power taken into account
// whether it is ran headless or not.
// peripherals????
// kernel???
// etc??
// Console.WriteLine("hi");

//IMPORTANT, I have to somewhere make a class that will only run once at the beginning of the game and continue to run for the entirety to serve as the backend data processing

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UIElements;
public static class Backend : MonoBehaviour
{
    [SerializeField]
    private static List<GameObject> termlist;
    private static List<int> activeIds;
    public static TermEnvironments termEnv = new TermEnvironments();
    private static int changedHash;
        void Start()
    {
        
    }
    void Update()
    {
        int currentHash = GetListHashCode<GameObject>(termlist);
        if (currentHash != changedHash)
        {
        }
    }
    public static string ProcessCommand(string input)
    {
        // All your terminal logic here
        if (input.Trim() == "exit") return null;
        if (input.Trim() == "help") return "Available: help, echo, math, exit";
        if (input.StartsWith("echo ")) return input.Substring(5);
        // ... etc
        return $"[error] Unknown command: {input}";
    }
    public static TermEnvironment FetchTerm(int id)
    {
        TermEnvironment returnval = new TermEnvironment();
        return returnval;
    }
    public static void SaveTermInfo(List<TermEnvironment> terms)
    {
        /* Hierarchy:
            list of terms:
            terms: objects containing 
         */
        foreach (TermEnvironment i in terms)
        {
            
        }
    }
    private static int GetListHashCode<T>(List<T> list)
    {
        int hash = 17;
        foreach (var item in list)
        {
            hash = hash * 23 * (item?.GetHashCode() ?? 0);
        }
        return hash;
    }

}

public class TermEnvironment
{
    public int processTime;
    public int memory;
    public int power;
    public int id {get; set;}
    private bool powered=false;
    public bool _powered => powered;
    private Memory ram;
    //the hard disks will be fetched using a generated name from the terminal id and maybe terminal name?, probably just id tho
    private List<HardMemory> HardSlots = new List<HardMemory>();
    private int SlotsAvailable=1;
    private int powerPerTick=1;

    public void PowerOn()
    {
        // have some logic here for how much power is needed to turn on and such
        if (power > 0)
        {
            powered = true;
        }
    }
    public void PowerOff()
    {
        powered = false;
    }
    public void RunTick()
    {
        if (powerPerTick <= power && powered)
        {
            power -= powerPerTick;
        }
        else if (powered)
        {
            powered = false;
            power = 0;
            //it's volatile!
            MemoryManager(0,0,"erase", new byte[0]);
        }
    }
    public byte[] MemoryManager(int index1, int index2, string modifyType, byte[] data)
    {
        //just like the typical, bytes work just like arrays (they are arrays), indexes starting at 0
        //Array.Copy
        //I don't think I will put in overflow
        switch (modifyType)
        {
            case "write":
                if (ram.Bytes.Length-index2>data.Length){
                    Array.Copy(data, index1, ram.Bytes, index2, data.Length);
                    return new byte[1];
                }
                return new byte[0];
            case "read":
                //prevent nonsensical indexies done, prevent the size obviously being too big done (needed?), prevent the read from going over the edge
                if (index2 > index1 && ram.Bytes.Length > index2 && index1 >= 0)
                {
                    byte[] returnBy = new byte[index2-index1+1];
                    Array.Copy(ram.Bytes, index1, returnBy, 0, index2-index1+1);
                    return returnBy;
                }
                return new byte[0];
            case "erase":
                ram.Bytes = new byte[ram.Size*1024];
                return new byte[1];
            default:
                return new byte[0];
        }
    }

    public bool StartUp()
    {


        //tempoerary so I don't have to look at the squiggly
        return true;
    }

}
public class Memory
{
    //in kib
    public int Size {get; set;}
    public byte[] Bytes {get; set;}
}

public class HardMemory
{
    public int Size {get; set;}
    public byte[] Bytes {get; set;}
    public bool encrypted=false;
    //might make this just an int later if I decide I just want to simplify it, this would just allow for most customizability
    public byte[] Passcodelock {get; set;}
    public string LocationOnDisk {get; set;}
}

public class TermEnvironments
{

    public List<TermEnvironment> terms;
    public TermEnvironment Fetch(int id)
    {
        //if the entry doesn't exist this WILL break
        TermEnvironment returnval = new TermEnvironment();
        returnval = terms.FirstOrDefault(i=>i.id==id);
        return returnval;
    }
    public bool termExists(int id)
    {
        // this needs to check if the term id has a saved value on disk
        return false;
    }

}
