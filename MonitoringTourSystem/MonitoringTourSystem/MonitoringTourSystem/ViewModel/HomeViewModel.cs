using MonitoringTourSystem.Infrastructures.EntityFramework;
using System.Collections.Generic;


namespace MonitoringTourSystem.ViewModel
{
    public class HomeViewModel
    {
        public List<TourIsProcessing> ListTourIsProcessing { get; set; }
        public List<WarningWithReceiver> ListWarningWithReceiver { get; set; }
        public int OptionRenderView { get; set; }
        public int NumberOfTourProcessing { get; set; }
        public int NumberOfSms { get; set; }
        public int NumberOfHelp { get; set; }

        public NotifyViewModel Notify { get; set; }

        public List<HelpViewModel> ListHelpWithTourInfo { get; set; }
    }

    public class TourIsProcessing
    {
        public tour Tour { get; set; }
        public tourguide TourGuide { get; set; }
    }
    public class WarningWithReceiver
    {
        public warning Warning { get; set; }
        public List<warning_receiver> ListWarningReceiver { get; set; }
        public int QuanityRecevied { get; set; }
    }

    public class HelpViewModel
    {
        public tourguide_help Help { get; set; }
        public tour TourInfo { get; set; }
    }

    public class NotifyViewModel
    {
        public List<notify> Notify { get; set; }
        public int CountNewNotify { get; set; }
    }
}