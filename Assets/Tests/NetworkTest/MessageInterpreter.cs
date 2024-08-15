using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageInterpreter
{
    // Singleton
    private static MessageInterpreter instance;

    public static MessageInterpreter Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new MessageInterpreter();
            }
            return instance;
        }
    }

    private MessageInterpreter()
    {
        interpreter_functions = new Dictionary<string, Func>();
    }
    
    delegate void Func(byte[] param, string user); 
    Dictionary<string,Func> interpreter_functions;

    public void Interpret(Message message)
    {
        interpreter_functions[message.Tag](message.Content, message.User);
    }
}
