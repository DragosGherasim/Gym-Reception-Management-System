using Gym_Reception_Management_System.ViewModels;

namespace Gym_Reception_Management_System.Utils
{
    public static class ViewModelLocator
    {
        public static MainWindowViewModel MainWindowVM { get; set; }
        public static LogInViewModel LoginInVM { get; set; }
        public static SignUpViewModel SignUpVM { get; set; }
        public static CreateMembershipViewModel CreateMembershipVM { get; set; }
        public static MembershipListViewModel MembershipListVM { get; set; }
        public static UpdateServiceViewModel UpdateServiceVM { get; set; }
        public static AddServiceViewModel AddServiceVM { get; set; }

        static ViewModelLocator()
        {
            MainWindowVM = new MainWindowViewModel();
            LoginInVM = new LogInViewModel();
            SignUpVM = new SignUpViewModel();
            CreateMembershipVM = new CreateMembershipViewModel();
            MembershipListVM = new MembershipListViewModel();
            UpdateServiceVM = new UpdateServiceViewModel();
            AddServiceVM = new AddServiceViewModel();
        }
    }
}