# CowsCannotReadLogs
An early stab at making a log file reader with customisable functionality.

This is an early alpha

## The problem solved

I was looking through log files and as usual I stretched my neck and squinted my eyes to understand what was happening.
As ususal the text was monochrome and the font boring with a fix width - 2 characterisics for a text mass that is hard to parse for the human eye.

So I thought to myself - why not create a simple program that takes 1 file and, with regex and C# code, orders the text in columns colourises it and visualises rows that should stick together.
So I wrote one such.

Presently (unless I have updated the source but forgotten to update this text (has happened before)) the only thing it can do is split a row into columns.
