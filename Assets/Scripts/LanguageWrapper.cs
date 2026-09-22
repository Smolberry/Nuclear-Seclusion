using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices.WindowsRuntime;
using NUnit.Framework.Constraints;
using NUnit.Framework.Interfaces;
using UnityEditor;
using UnityEngine.Rendering.Universal;

/// <summary>
/// from the start look for a type (of method) declaration
/// after detecting a delimiter of () or {} look at the previous words to decide on what kind of method and then look forward in the lines
/// to see exactly what is contained within it and if it fits the syntax/should error
/// 
/// while within delimiters the syntax of what should be done should change
/// within () look for argument variables, I'm not entirely sure if I should make it so that you have to declare what type it is prior to execution or not
/// within {} look for statements that must be executed
/// 
/// ultimately will have to create an isolated environment where this code will be ran and values for each method will be created.
/// functions will have their own wrapper
/// </summary>



public class LanguageWrapper
{
    public String value;
    public String type;
    public LanguageWrapper(String arg, String type)
    {
        this.value = arg;
        this.type = type;
    }
    /// <summary>
    /// I think I might depreciate this
    /// </summary>
    public static LanguageWrapper[] UnwrapLang(String[] args)
    {
        LanguageWrapper[] returnval = new LanguageWrapper[args.Length];
        // if wrapped in "" its a string, integer, double and float will be the same, idk if I am going to make declarations or not
        foreach (string i in args)
        {
            if (i[0]=='"' && i[-1]=='"' || i[0]=='"' && i[-1]=='"')
            {
                LanguageWrapper temp= new LanguageWrapper(i, "string");
                returnval.Append(temp);
            }
            else if (float.TryParse(i, out _))
            {
                returnval.Append(new LanguageWrapper(i, "float"));
            }
            else if (int.TryParse(i, out _))
            {
                returnval.Append(new LanguageWrapper(i, "int"));
            }
            else if (i[0]=='"'&&i[-1]=='"')
            {
                throw new ArgumentException(nameof(i), "type not able to be processed");
            }
        }
        return returnval;
    }
    public static String getType(String val)
    {
        if (val[0]=='"' && val[-1]=='"' || val[0]=='"' && val[-1]=='"')
        {
            return "string";

        }
        else if (float.TryParse(val, out _))
        {
            return "float";
        }
        else if (int.TryParse(val, out _))
        {
            return "integer";
        }
        else if (val[0]=='['&&val[-1]==']')
        {
            return "list";
        }
        else if (val[0]=='{'&&val[-1]=='}')
        {
            return "dictionary";
        }
        else if (val[0]=='('&&val[-1]==')')
        {
            return "tuple";
        }
        else if (val.ToLower()=="true"||val.ToLower()=="false")
        {
            return "boolean";
        }
        else
        {
            throw new ArgumentException(nameof(val), "type not able to be processed");
        }
    }
}
public class LineWrapper
{
    public String line;
    public String operation;
    public int indentation;
    public LineWrapper(String arg, String operation, int indentation)
    {
        this.line = arg;
        this.operation=operation;
        this.indentation=indentation;
    }
    public static LineWrapper[] LineProcessor(String[] arglist)
    {
        int length = 0;
        LineWrapper previous;
        int indentation;
        List<LineWrapper> templist = new List<LineWrapper>();
        String oper = "Error";
        Char[] AllDelimiters = {'{', '}', '(', ')', '.',';'};

        foreach (String arg in arglist)
        {
            indentation = 0;
            int delimiters = 0;
            if (arg[0]=='\t' || arg[0]==' ')
            {
                indentation+=1;
                int thing = 1;
                // what the fuck was I doing here
                foreach(arg[thing]=='\t' ||  arg[thing]==' ')
                {
                    indentation+=1;
                    thing+=1;
                }

            }
            if (delimiters < 2)
            {
                
            }

            //this is just the end result there will be more checks before this
            length+=1;
            templist.Append(new LineWrapper(arg, oper, indentation));
        }
        LineWrapper[] returnval = new LineWrapper[length];
        foreach (LineWrapper t in templist)
        {
            returnval.Append(t);
        }
        return returnval;
    }
}
/// <summary>
/// go through each line figuring out what is what and categorize/index everything
/// I DO NOT need to make a wrapper for args, I plan to just insert them into the functions where they can be inserted.
///
/// </summary>
public class FuncWrapper
{
    String name;
    String expression;
    public FuncWrapper(String funcname, String expression)
    {
        this.name = funcname;
        this.expression = expression;
    }
    public static FuncWrapper ProcessFunction(String[] lines)
    {
        String name;
        String expression;
        int processedindex=0;
        if (!(lines[0].StartsWith("function")))
        {
            throw new ArgumentException("Value Error", "this is not a function");
        }
        // foreach (String linePart in lines[0].Split(" "))
        // {
        //     if (linePart !="\t")
        //     {
        //         if (processedindex==1 && (linePart !=" " | linePart !="\t"))
        //         {
        //             name = linePart;
        //         }
        //         //end of this foreach loop
        //         processedindex+=1;
        //     }
        // }
        String previous;
        FuncWrapper tempfunc;
        String[] args;
        foreach (String line in lines)
        {
            /// initial idea: look for everything on the left first to figure out where things are all at, then go through again to the areas
            /// that are known and catalogue everything
            switch (fetchIndice(line, 0))
            {
                case "function":
                    previous = "function";
                    tempfunc = new FuncWrapper(fetchIndice(line, 1), "");
                    args = fetchArgs(line);
                    break;
                case "int":

                    break;
            }
        }


        return new FuncWrapper(name, expression);
    }
    public static String fetchIndice(String line, int ind)
    {
        String result = "";
        int loop = 0;
        foreach(Char a in line)
        {
            if (Char.IsLetterOrDigit(a) && loop == ind)
            {
                result += a;
            }
            else if (loop == ind)
            {
                return result;
            }
        }
        return result;
    }
    public static String[] fetchArgs(String argLine)
    {
        String affectedLine = "";
        Boolean reached = false;
        foreach (char c in argLine)
        {
            if (!reached && c=='(')
            {
                reached = true;
            }
            else if (c==')')
            {
                break;
            }
            else
            {
                affectedLine += c;
            }
        }
        return affectedLine.Split(',', StringSplitOptions.RemoveEmptyEntries);
    }
    public static Dictionary<String,String> fetchVars(String varLine)
    {
        Boolean inDelimiter = false;
        String storage = "";
        int first = 0;
        string name="";
        string othershit;
        Dictionary<String,String> result = new Dictionary<String, String>();
        foreach (String s in varLine.Split(','))
        {

             if (s.Contains('='))
            {
                if (first == 0)
                {
                    String[] r = s.Split('=');
                    first+=1;
                    name = r[0];
                    if (r[1].Contains("("))
                    {
                        inDelimiter = true;
                        storage+=r[1]+',';
                    }
                }
            }
            if (inDelimiter)
            {
                if (s.Contains(")") && storage.Count(f=>f==')' || f=='(') + s.Count(f=>f==')' || f=='(') % 2 < 1)
                {
                    inDelimiter = false;
                    result[name] = storage+s;
                    storage = "";
                    name = "";
                    first = 0;
                }
                else
                {
                    storage+=s+',';
                }
                // else
                // {
                //     // first off what the fuck?
                //     throw new ArgumentException(nameof(s), "unexpected input??? idek how this would happen")
                // }
            }
        }
        return result;
    }
}
