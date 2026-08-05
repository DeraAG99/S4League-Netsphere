//using System;
//using System.Collections.Generic;
//using System.Globalization;
//using System.Linq;
//using System.Reflection;

//namespace BlubLib.Configuration
//{
//    public static class CommandLineParser
//    {
//        public static bool Parse(string[] args, object options)
//        {
//            var type = options.GetType();
//            var props = type.GetProperties();
//            var cmdAttrib = GetCommandLineAttributes(props);
//            var cmdValueAttrib = GetCommandLineValueAttributes(props).ToArray();

//            var cmdObjects = GetObjects(args);
//            if (cmdObjects == null)
//                return false;

//            //var cmdObj = cmdObjects.FirstOrDefault(e => e.Name != null && 
//            //    (e.Name.Equals("help", StringComparison.InvariantCultureIgnoreCase) || e.Name.Equals("h", StringComparison.InvariantCultureIgnoreCase)));
//            //if (cmdObj != null)
//            //{
//            //    if (cmdObj.Value == null)
//            //    {
//            //        options.PrintHelp();
//            //    }
//            //    else
//            //    {
//            //        var res = cmdAttrib.FirstOrDefault(e =>
//            //        {
//            //            var attrib = (CommandLineAttribute) e.Attribute;
//            //            return attrib.ShortCmd != null &&
//            //                   attrib.ShortCmd.Equals(cmdObj.Value, StringComparison.InvariantCultureIgnoreCase) ||
//            //                   attrib.Cmd != null &&
//            //                   attrib.Cmd.Equals(cmdObj.Value, StringComparison.InvariantCultureIgnoreCase);
//            //        });
//            //        options.PrintCommandHelp(((CommandLineAttribute)res.Attribute).Help);
//            //    }
//            //}

//            try
//            {
//                if (!SetOptions(options, cmdObjects, cmdAttrib))
//                    return false;
//                if (!SetValues(options, cmdObjects, cmdValueAttrib))
//                    return false;
//            }
//            catch (Exception)
//            {
//                return false;
//            }

//            return true;
//        }

//        private static bool SetOptions(object options, CommandLineObject[] cmdObjects, IEnumerable<CommandLinePropertyObject> propertyObjects)
//        {
//            foreach (var item in propertyObjects)
//            {
//                var attrib = (CommandLineAttribute)item.Attribute;
//                var cmdObj = cmdObjects.FirstOrDefault(e =>
//                {
//                    if (e.Name == null)
//                        return false;
//                    if (e.IsFullName)
//                    {
//                        return attrib.Cmd != null && attrib.Cmd.Equals(e.Name, StringComparison.InvariantCultureIgnoreCase);
//                    }
//                    else
//                    {
//                        return attrib.ShortCmd != null && attrib.ShortCmd.Equals(e.Name);
//                    }
//                });
//                if (cmdObj == null)
//                {
//                    if (attrib.Required)
//                        return false;
//                    continue;
//                }

//                if (cmdObj.Value == null)
//                {
//                    if (item.PropertyInfo.PropertyType != typeof(bool))
//                        return false;
//                    item.PropertyInfo.SetValue(options, true);
//                }
//                else
//                {
//                    if (item.PropertyInfo.PropertyType == typeof(string))
//                    {
//                        item.PropertyInfo.SetValue(options, cmdObj.Value);
//                    }
//                    else
//                    {
//                        try
//                        {
//                            var value = Convert.ChangeType(cmdObj.Value, item.PropertyInfo.PropertyType, CultureInfo.InvariantCulture);
//                            item.PropertyInfo.SetValue(options, value);
//                        }
//                        catch (Exception)
//                        {
//                            return false;
//                        }
//                    }
//                }
//            }
//            return true;
//        }
//        private static bool SetValues(object options, CommandLineObject[] cmdObjects, CommandLinePropertyObject[] propertyObjects)
//        {
//            var j = 0;
//            foreach (var cmdObj in cmdObjects)
//            {
//                if (cmdObj.Name != null)
//                    continue;

//                var prop = propertyObjects.FirstOrDefault(e => ((CommandLineValueAttribute) e.Attribute).Index == j);
//                if (prop == null)
//                    return false;

//                if (prop.PropertyInfo.PropertyType == typeof(string))
//                {
//                    prop.PropertyInfo.SetValue(options, cmdObj.Value);
//                }
//                else
//                {
//                    try
//                    {
//                        var value = Convert.ChangeType(cmdObj.Value, prop.PropertyInfo.PropertyType, CultureInfo.InvariantCulture);
//                        prop.PropertyInfo.SetValue(options, value);
//                    }
//                    catch (Exception)
//                    {
//                        return false;
//                    }
//                }
//                j++;
//            }

//            return true;
//        }

//        private static CommandLineObject[] GetObjects(string[] args)
//        {
//            var ls = new List<CommandLineObject>();

//            for (var i = 0; i < args.Length; i++)
//            {
//                var current = args[i];
//                CommandLineObject obj;

//                if (current.StartsWith("--"))
//                {
//                    #region FullName

//                    obj = new CommandLineObject
//                    {
//                        IsFullName = true,
//                        Name = current.Substring(2)
//                    };

//                    if (current.Contains("="))
//                    {
//                        var arr = obj.Name.Split(new[] {"="}, StringSplitOptions.RemoveEmptyEntries);
//                        obj.Name = arr[0];
//                        if (arr.Length > 1)
//                            obj.Value = arr[1];
//                    }
//                    else if (current.Contains(":"))
//                    {
//                        var arr = obj.Name.Split(new[] {":"}, StringSplitOptions.RemoveEmptyEntries);
//                        obj.Name = arr[0];
//                        if (arr.Length > 1)
//                            obj.Value = arr[1];
//                    }
//                    else
//                    {
//                        var value = args[i + 1];
//                        if (!value.StartsWith("-"))
//                        {
//                            obj.Value = value;
//                            i++;
//                        }
//                    }

//                    #endregion
//                }
//                else if (current.StartsWith("-"))
//                {
//                    #region ShortName

//                    obj = new CommandLineObject
//                    {
//                        IsFullName = false,
//                        Name = current.Substring(1)
//                    };

//                    if (current.Contains("="))
//                    {
//                        var arr = obj.Name.Split(new[] {"="}, StringSplitOptions.RemoveEmptyEntries);
//                        obj.Name = arr[0];
//                        if (arr.Length > 1)
//                            obj.Value = arr[1];
//                    }
//                    else if (current.Contains(":"))
//                    {
//                        var arr = obj.Name.Split(new[] {":"}, StringSplitOptions.RemoveEmptyEntries);
//                        obj.Name = arr[0];
//                        if (arr.Length > 1)
//                            obj.Value = arr[1];
//                    }
//                    else
//                    {
//                        if (i + 1 < args.Length)
//                        {
//                            var value = args[i+1];
//                            if (!value.StartsWith("-"))
//                            {
//                                obj.Value = value;
//                                i++;
//                            }
//                        }
//                    }

//                    #endregion
//                }
//                else
//                {
//                    obj = new CommandLineObject
//                    {
//                        IsFullName = false,
//                        Value = current
//                    };
//                }
//                ls.Add(obj);
//            }

//            return ls.ToArray();
//        }

//        private static IEnumerable<CommandLinePropertyObject> GetCommandLineAttributes(IEnumerable<PropertyInfo> propertyInfos)
//        {
//            foreach (var propertyInfo in propertyInfos)
//            {
//                var attrib = propertyInfo.GetCustomAttributes(typeof(CommandLineAttribute)).FirstOrDefault();
//                if (attrib == null)
//                    continue;

//                var obj = new CommandLinePropertyObject
//                {
//                    Attribute = attrib,
//                    PropertyInfo = propertyInfo
//                };

//                yield return obj;
//            }
//        }
//        private static IEnumerable<CommandLinePropertyObject> GetCommandLineValueAttributes(IEnumerable<PropertyInfo> propertyInfos)
//        {
//            foreach (var propertyInfo in propertyInfos)
//            {
//                var attrib = propertyInfo.GetCustomAttributes(typeof(CommandLineValueAttribute)).FirstOrDefault();
//                if (attrib == null)
//                    continue;

//                var obj = new CommandLinePropertyObject
//                {
//                    Attribute = attrib,
//                    PropertyInfo = propertyInfo
//                };

//                yield return obj;
//            }
//        }

//        private class CommandLineObject
//        {
//            public bool IsFullName { get; set; }

//            public string Name { get; set; }
//            public string Value { get; set; }
//        }

//        private class CommandLinePropertyObject
//        {
//            public Attribute Attribute { get; set; }
//            public PropertyInfo PropertyInfo { get; set; }
//        }
//    }

//    [AttributeUsage(AttributeTargets.Property)]
//    public class CommandLineAttribute : Attribute
//    {
//        // ReSharper disable FieldCanBeMadeReadOnly.Local
//        // ReSharper disable ConvertToAutoPropertyWithPrivateSetter
//        private string _shortCmd;
//        private string _cmd;

//        //public string Help { get; set; }
//        public string ShortCmd
//        {
//            get { return _shortCmd; }
//        }
//        public string Cmd
//        {
//            get { return _cmd; }
//        }
//        public bool Required { get; set; }

//        public CommandLineAttribute(string shortCmd, string cmd)
//        {
//            if (string.IsNullOrWhiteSpace(shortCmd) && string.IsNullOrWhiteSpace(cmd))
//                throw new ArgumentException("shortCmd or cmd must contain a value");

//            //Help = "";
//            _shortCmd = shortCmd;
//            _cmd = cmd;
//        }

//        // ReSharper restore ConvertToAutoPropertyWithPrivateSetter
//        // ReSharper restore FieldCanBeMadeReadOnly.Local
//    }

//    [AttributeUsage(AttributeTargets.Property)]
//    public class CommandLineValueAttribute : Attribute
//    {
//        // ReSharper disable FieldCanBeMadeReadOnly.Local
//        // ReSharper disable ConvertToAutoPropertyWithPrivateSetter
//        private int _index;

//        public int Index
//        {
//            get { return _index; }
//        }

//        public CommandLineValueAttribute(int index)
//        {
//            _index = index;
//        }

//        // ReSharper restore ConvertToAutoPropertyWithPrivateSetter
//        // ReSharper restore FieldCanBeMadeReadOnly.Local
//    }
//}
