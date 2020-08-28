using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Security.Credentials.UI;

namespace GetSystemInfo
{
    //MIDL_INTERFACE("39E050C3-4E74-441A-8DC0-B81104DF949C")
    //IUserConsentVerifierInterop : public IInspectable
    //{
    //public:
    //    virtual HRESULT STDMETHODCALLTYPE RequestVerificationForWindowAsync(
    //        /* [in] */ HWND appWindow,
    //        /* [in] */ HSTRING message,
    //        /* [in] */ REFIID riid,
    //        /* [iid_is][retval][out] */ void** asyncOperation) = 0;
    //};
    [Guid("39E050C3-4E74-441A-8DC0-B81104DF949C")]
    [InterfaceType(ComInterfaceType.InterfaceIsIInspectable)]
    public interface IUserConsentVerifierInterop
    {
        IAsyncOperation<UserConsentVerificationResult> RequestVerificationForWindowAsync(IntPtr appWindow, [MarshalAs(UnmanagedType.HString)] string Message, [In] ref Guid riid);
    }

    //Helper to initialize UserConsentVerifier
    public static class UserConsentVerifierInterop
    {
        public static IAsyncOperation<UserConsentVerificationResult> RequestVerificationForWindowAsync(IntPtr hWnd, string Message)
        {
#if NETCOREAPP3_0
            IUserConsentVerifierInterop userConsentVerifierInterop = (IUserConsentVerifierInterop)WindowsRuntimeMarshal.GetActivationFactory(typeof(UserConsentVerifier));
#else
            //IUserConsentVerifierInterop userConsentVerifierInterop = new WinRT.ActivationFactory<UserConsentVerifier>().As<Windows.Foundation.IGetActivationFactory>().As<IUserConsentVerifierInterop>();
            //IUserConsentVerifierInterop userConsentVerifierInterop = UserConsentVerifier.As<Windows.Foundation.IGetActivationFactory>().As<IUserConsentVerifierInterop>();
#endif
            Guid guid = typeof(IAsyncOperation<UserConsentVerificationResult>).GUID;
            
            return userConsentVerifierInterop.RequestVerificationForWindowAsync(hWnd, Message, ref guid);

        }
    }
}
