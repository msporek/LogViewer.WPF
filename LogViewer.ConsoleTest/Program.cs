﻿using LogViewer.Base;
using LogViewer.Base.Models;
using LogViewer.Base.Parsers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

Console.WriteLine("Hello, World!");

try
{
    string[] allLines = await File.ReadAllLinesAsync("85C27NK92C.com.flexibits.fantastical2.mac.helper 2022-11-26--03-25-05-612.log");

    Queue<LogEntry> logEntries = new Queue<LogEntry>(new LogSplitter().SplitEntries(allLines));

    List<ILogEntryParser> logEntryParsers =
        new List<ILogEntryParser>()
        {
            new AccountLogEntryParser(),
            new CalendarLogEntryParser(),
            new CurrentCalendarSetLogEntryParser(), 
            new SyncQueuesLogEntryParser(),
        };

    /* 
     * One can use events to subscribe to parser operations if they wish. 
     * 
    foreach (ILogEntryParser parser in logEntryParsers)
    {
        parser.LinesParsed += (sender, lpea) => allLogEntries.AddRange(lpea.Entries);
    }
    */

    List<LogItem> allLogItems = new List<LogItem>();
    while (logEntries.Any())
    {
        bool lineParsed = false;
        foreach (ILogEntryParser parser in logEntryParsers)
        {
            if (parser.TryParse(logEntries, out var newlyParsedEntries))
            {
                lineParsed = true;
                allLogItems.AddRange(newlyParsedEntries);

                break;
            }
        }

        if (!lineParsed)
        {
            logEntries.Dequeue();
        }
    }

    Console.WriteLine("All lines parsed.");

    foreach (LogItem logItem in allLogItems)
    {
        Console.WriteLine($"{logItem.GetType()}: {logItem.Contents}");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Error occured: {ex}");
}