using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TwoFactAuthentication.Models;

//inastall twilio library and include below namespaces

namespace TwoFactAuthentication.Controllers
{
    public class TwoFactAuthController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public TwoFactAuthController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }
 
        //on click of Generate OTP,call SendOTP,OTP will be sent
//https://www.youtube.com/watch?v=xO7xUVqBEiw
        public JsonResult SendOTP()
        {int otpValue = new Random().Next(100000, 999999);
            var status = "";
            try
            {
                string recipient =  ConfigurationManager.AppSettings["RecipientNumber"].ToString();
                string APIKey = ConfigurationManager.AppSettings["APIKey"].ToString();

                string message = "Your OTP Number is " + otpValue + " ( Sent By : Technotips-Ashish )";
                String encodedMessage = System.Web.HttpUtility.UrlEncode(message);

                using (var webClient = new WebClient())
                {
                    byte[] response = webClient.UploadValues("https://api.textlocal.in/send/", new NameValueCollection(){
                                        
                                         {"apikey" , APIKey},
                                         {"numbers" , recipient},
                                         {"message" , encodedMessage},
                                         {"sender" , "TXTLCL"}});

                    string result = System.Text.Encoding.UTF8.GetString(response);

                    var jsonObject = JObject.Parse(result);

                    status = jsonObject["status"].ToString();

                    Session["CurrentOTP"] = otpValue;
                }


                return Json(status, JsonRequestBehavior.AllowGet);


            }
            catch (Exception e)
            {

                throw (e);

            }
        }

        
        //call verifyotp on verify button click
        public ActionResult VerifyOTP(LoginViewModel loginViewModel)
        {
            if(loginViewModel.OTPentered==session("CurrOTP").toString())
            {
                //login the user using identity classes 
                
                // redirect to home
                        return RedirectToAction("Home");
            }
            
            //add errmessage to LoginAction Summary
            return View("LoginView",loginViewModel);
            
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
