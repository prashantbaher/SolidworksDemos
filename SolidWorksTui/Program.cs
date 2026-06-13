using SolidWorksTui.Views;
using Terminal.Gui.App;

var app = Application.Create();
app.Init("");

try
{
    var mainView = new MainView(app);
    app.Run(mainView);
}
finally
{
    ((IDisposable)app).Dispose();
}
