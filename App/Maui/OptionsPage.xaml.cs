using static qr2web.Options;

namespace qr2web;

public partial class OptionsPage : ContentPage
{
    readonly Dictionary<Options.BarcodeType, Switch> Switches = [];
    readonly List<Options.KeyboardType> PickerItems = [];

	public OptionsPage()
	{
		InitializeComponent();

		Switches.Add(Options.BarcodeType.Code, barCodeTypeCode);
        Switches.Add(Options.BarcodeType.Ean, barCodeTypeEan);
        Switches.Add(Options.BarcodeType.Upc, barCodeTypeUpc);
        Switches.Add(Options.BarcodeType.Other1d, barCodeTypeOther1d);

		Switches.ToList().ForEach((p) =>
		{
			p.Value.IsToggled = Options.Barcodes[p.Key];
		});

		homePage.Text = Options.HomePage;
		forcePortrait.IsToggled = Options.ForcePortrait;
        showScan.IsToggled = Options.ShowScanbutton;
        forwardLocation.IsToggled = Options.ForwardLocation;
        useHistory.IsToggled = Options.UseHistory;
		forceBackground.IsToggled = Options.ForceBackground;
        allowExternal.IsToggled = Options.ExternalLinks;

        Enum.GetValues(typeof(Options.KeyboardType)).Cast<Options.KeyboardType>().ToList().ForEach(kt =>
        {
            PickerItems.Add(kt);
            keyboardType.Items.Add(kt.ToString());
            if (Options.Keyboard == kt) keyboardType.SelectedItem = kt.ToString();
        });

        Enum.GetValues(typeof(Options.Languages)).Cast<Options.Languages>().ToList().ForEach(l =>
        {
            language.Items.Add(l.ToString());
            if (Options.Language == l.ToString().ToLower()[..2]) language.SelectedItem = l.ToString();
        });
    }

    protected override void OnDisappearing()
    {
		Options.SaveHomePage(homePage.Text);

        Switches.ToList().ForEach(p =>
            Options.ActivateBarcode(p.Key, p.Value.IsToggled));

        Options.SetForcePortrait(forcePortrait.IsToggled);
        Options.SetShowScanButton(showScan.IsToggled);
        Options.SetUseHistory(useHistory.IsToggled);
        Options.SetForwardLocation(forwardLocation.IsToggled);
        Options.SetForceBackground(forceBackground.IsToggled);
        Options.SetExternalLinks(allowExternal.IsToggled);
        Options.SetKeyboardType(PickerItems[keyboardType.SelectedIndex]);
        string? newLang = language.SelectedItem.ToString();
        newLang ??= "sy";
        Options.SetLanguage(newLang);

        base.OnDisappearing();
    }
}