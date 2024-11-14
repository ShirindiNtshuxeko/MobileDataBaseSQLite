using MobileDataBaseSQLite.ViewModel;
using MobileDataBaseSQLite.Model;
using static MobileDataBaseSQLite.Model.ModelCustomers;



namespace MobileDataBaseSQLite.View;

public partial class ViewPage : ContentPage
{
	public ViewPage()
	{
		InitializeComponent();

        BindingContext = new ViewModelSave(this);
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        // Subscribe to the messaging center to show alerts
        MessagingCenter.Subscribe<ViewModelSave, string>(this, "DisplayAlert", async (sender, message) =>
        {
            // Show the alert when the message is received
            await DisplayAlert("Message", message, "OK");
        });
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        // Unsubscribe from the MessagingCenter to prevent memory leaks
        MessagingCenter.Unsubscribe<ViewModelSave, string>(this, "DisplayAlert");
    }

    private async void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
    {
        var viewModel = (ViewModelSave)BindingContext;
        await viewModel.OnCustomerTapped((Customer)e.Item);
    }


}