// See https://aka.ms/new-console-template for more information

using Presentation_Layer;

Console.WriteLine("Hello World");
DataPresentationLayer testobject = DataPresentationLayer.Instance;

testobject.activateDatabase(@"C:\Users\unruh\Desktop\TestDB", "TestDB", "1234");

//testobject.NewDataBase("Testi", "Test1234", DataPresentationLayer.DataBasetype.Accounting, ["Test1", "Test2"], ["Test1"]);

Dictionary<string, string> Testdata = new Dictionary<string, string>();
Testdata.Add("Ammount", "123,24");
Testdata.Add("DebitAccount", "1200");
Testdata.Add("CreditAccount", "4980");
Testdata.Add("Date", "11.11.1111");
Testdata.Add("Test1", "TestallerTests");
Testdata.Add("Test2", "TestallerTests2");
testobject.Create("Testi", Testdata);
var test = testobject.ReadAccount("Testi", "1200");
Console.WriteLine(test);
Dictionary<string,string> updateTestdate = new Dictionary<string, string>();
updateTestdate.Add("Ammount", "123,24");
updateTestdate.Add("DebitAccount", "1200");
updateTestdate.Add("CreditAccount", "4980");
updateTestdate.Add("Date", "11.11.1111");
updateTestdate.Add("Test1", "Wert ersetzt");
updateTestdate.Add("Test2", "Wert ersetzt");
testobject.Update("Testi", updateTestdate, "2");
testobject.Delete("Testi", "2");

