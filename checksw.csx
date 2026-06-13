using System.Reflection;
var asm = typeof(System.Runtime.InteropServices.Marshal).Assembly;
Console.WriteLine(asm.FullName);
var methods = typeof(System.Runtime.InteropServices.Marshal).GetMethods(BindingFlags.Public | BindingFlags.Static)
    .Where(m => m.Name.Contains("Active") || m.Name.Contains("GetObject"));
foreach (var m in methods)
    Console.WriteLine($"  {m.Name}");
