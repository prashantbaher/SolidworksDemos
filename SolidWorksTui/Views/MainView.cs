using Terminal.Gui.App;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;

namespace SolidWorksTui.Views;

public class MainView : Window
{
    private readonly IApplication _app;
    private readonly Label _statusLabel;

    public MainView(IApplication app)
    {
        _app = app;
        Title = "SolidWorks TUI";
        Width = Dim.Fill();
        Height = Dim.Fill();

        var menuBar = new MenuBar([
            new MenuBarItem("_File", [
                new MenuItem("_Open", "Open file", ShowHello, null),
                new MenuItem("_Exit", "Exit", () => _app.RequestStop(), null)
            ])
        ]);

        _statusLabel = new Label
        {
            Text = "Press Alt+F, then O to see a message",
            X = Pos.Center(),
            Y = Pos.Center(),
            Width = Dim.Fill()
        };

        Add(menuBar, _statusLabel);
    }

    private void ShowHello()
    {
        _statusLabel.Text = "Hello World!";
        MessageBox.Query(_app, "Message", "Hello World!", "OK");
    }
}
