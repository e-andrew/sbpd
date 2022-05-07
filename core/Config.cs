﻿using System.Collections;
using System.IO;
using System.Text;

namespace Cerberus.core
{
    public class Config
    {
        private readonly string _path;
        private readonly char _lineDelimiter = '\n';
        private readonly char _columnDelimiter = ',';
        
        public Config(string path)
        {
            _path = path + ".csv";
            if (!File.Exists(_path))
            {
                File.Create(_path).Close();
            }
        }
        private string BuildRow(string[] data)
        {
            StringBuilder row = new StringBuilder();
            int last = data.Length - 1;
            for (int i = 0; i < last; i++)
            {
                row.Append(data[i]).Append(_columnDelimiter);
            }
            row.Append(data[last]);
            return row.ToString();
        }
        
        public void Insert(string[] record)
        {
            StringBuilder rows = new StringBuilder();
            rows.Append(BuildRow(record)).Append(_lineDelimiter);
            StreamWriter streamWriter = new StreamWriter(_path, true);
            streamWriter.Write(rows);
            streamWriter.Close();
        }
        public void InsertAll(ArrayList records)
        {
            StringBuilder rows = new StringBuilder();
            
            foreach (string[] record in records)
            {
                rows.Append(BuildRow(record)).Append(_lineDelimiter);
            }
            
            StreamWriter streamWriter = new StreamWriter(_path, true);
            streamWriter.Write(rows);
            streamWriter.Close();
        }
        public string[] FindRecord(string value, int column, string[] empty)
        {
            foreach (string[] record in FindAll())
            {
                if (record[column] == value)
                {
                    return record;
                } 
            }
            
            return empty;
        }
        public ArrayList FindAll()
        {
            StreamReader streamReader = new StreamReader(_path);
            string[] rows = streamReader.ReadToEnd().Split(_lineDelimiter);
            streamReader.Close();
            ArrayList data = new ArrayList();
            for (int i = 0; i < rows.Length - 1; i++)
            {
                data.Add(rows[i].Split(_columnDelimiter));
            }
            return data;
        }
        
        public void Exclude(string value, int column)
        {
            ArrayList records = FindAll();
            foreach (string[] record in records)
            {
                if (record[column] == value)
                {
                    records.Remove(record);
                    break;
                }
            }
            StreamWriter streamWriter = new StreamWriter(_path, false);
            streamWriter.Close();
            InsertAll(records);
        }
        
        public void ExcludeAll(string value, int column)
        {
            ArrayList records = new ArrayList();
            foreach (string[] record in FindAll())
            {
                if (record[column] != value)
                {
                    records.Add(record);
                }
            }
            StreamWriter streamWriter = new StreamWriter(_path, false);
            streamWriter.Close();
            InsertAll(records);
        }

        public void Delete()
        {
            File.Delete(_path);
        }

        public int Count(string[] values, int[] columns)
        {
            int count = 0;
            
            foreach (string[] record in FindAll())
            {
                bool decision = true;
                for (int i = 0; i < values.Length; i++)
                {
                    decision = decision && record[columns[i]] == values[i];
                }
                
                if (decision) count++;
            }
            
            return count;
        }
        public int GetSize()
        {
            StreamReader streamReader = new StreamReader(_path);
            int size = streamReader.ReadToEnd().Split(_lineDelimiter).Length;
            streamReader.Close();
            return size - 1;
        }
        
    }
}