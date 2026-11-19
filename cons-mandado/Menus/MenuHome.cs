using VoiceControl;
using VsConsole;

public class MenuHome(IVoiceInput _voiceInput) : AMenu
{
    protected override void SetupCommands()
    {
        //AddCommand(CaptureTextToMsg);
        AddShortCommand(ConsoleKey.V, CaptureTextToMsg);
    }

    /// <summary>
    /// Gestionnaire de la touche V : <see cref="AMenu.AddShortCommand"/> impose une signature
    /// <see cref="Action"/> (void), d'où le async void. Le wrapper ne fait qu'appeler la méthode
    /// Task ci-dessous et capte toute exception résiduelle : dans un async void, une exception
    /// non capturée crasherait le process.
    /// </summary>
    private async void CaptureTextToMsg()
    {
        try
        {
            await CaptureTextToMsgAsync();
        }
        catch (Exception ex)
        {
            AddFooterMessage($"Erreur saisie vocale : {ex.Message}");
            DisplayPage();
        }
    }

    private async Task CaptureTextToMsgAsync()
    {
        using CancellationTokenSource cts = new ();
        // Watcher clavier : la touche O annule le token = "fin de saisie".
        // Note : cts.Cancel() n'annule pas le ReadKey bloquant, ce thread reste
        // suspendu jusqu'à la prochaine frappe clavier.
        _ = Task.Run(() =>
        {
            if (Console.ReadKey(intercept: true).Key == ConsoleKey.O)
            {
                cts.Cancel();
            }
        });

        AddFooterMessage("Enregistrement en cours - o pour arreter");
        this.DisplayPage();

        try
        {
            string msg = await _voiceInput.CaptureTextAsync(cts.Token);
            AddFooterMessage(string.IsNullOrWhiteSpace(msg) ? "Rien compris" : msg);
        }
        catch (OperationCanceledException)
        {
            AddFooterMessage("Saisie vocale annulée");
        }
        finally
        {
            cts.Cancel(); // stoppe la capture (fin de saisie)
            this.DisplayPage();
        }
    }
}
