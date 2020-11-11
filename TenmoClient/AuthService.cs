using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using TenmoClient.Data;

namespace TenmoClient
{
    public class AuthService
    {
        private readonly static string API_BASE_URL = "https://localhost:44315/";
        private readonly IRestClient client = new RestClient();

        public string Token { get; set; }

        //login endpoints
        public bool Register(LoginUser registerUser)
        {
            RestRequest request = new RestRequest(API_BASE_URL + "login/register");
            request.AddJsonBody(registerUser);
            IRestResponse<API_User> response = client.Post<API_User>(request);

            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                throw new Exception("An error occurred communicating with the server.");
            }
            else if (!response.IsSuccessful)
            {
                if (!string.IsNullOrWhiteSpace(response.Data.Message))
                {
                    throw new Exception("An error message was received: " + response.Data.Message);
                }
                else
                {
                    throw new Exception("An error response was received from the server. The status code is " + (int)response.StatusCode);
                }
            }
            else
            {
                return true;
            }
        }

        public API_User Login(LoginUser loginUser)
        {
            RestRequest request = new RestRequest(API_BASE_URL + "login");
            request.AddJsonBody(loginUser);
            IRestResponse<API_User> response = client.Post<API_User>(request);

            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                throw new Exception("An error occurred communicating with the server.");

            }
            else if (!response.IsSuccessful)
            {
                if (!string.IsNullOrWhiteSpace(response.Data.Message))
                {
                    throw new Exception("An error message was received: " + response.Data.Message);
                }
                else
                {
                    throw new Exception("An error response was received from the server. The status code is " + (int)response.StatusCode);
                }

            }
            else
            {
                client.Authenticator = new JwtAuthenticator(response.Data.Token);
                this.Token = response.Data.Token;
                return response.Data;
            }
        }

        public double GetBalance(int userId)
        {
            RestRequest request = new RestRequest(API_BASE_URL + "user/balance/" + userId);
            IRestResponse<double> response = client.Get<double>(request);

            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                throw new Exception("An error occurred communicating with the server.");

            }
            else if (!response.IsSuccessful)
            {
                throw new Exception("An error response was received from the server. The status code is " + (int)response.StatusCode);
            }

            return response.Data;
        }

        public List<API_User> GetUsers()
        {
            RestRequest request = new RestRequest(API_BASE_URL + "user/list");
            IRestResponse<List<API_User>> response = client.Get<List<API_User>>(request);

            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                throw new Exception("An error occurred communicating with the server.");
            }
            else if (!response.IsSuccessful)
            {
                throw new Exception("An error response was received from the server. The status code is " + (int)response.StatusCode);
            }

            return response.Data;
           
        }

        public bool TransferMoney(TransferMoney transfer, int userId)
        {
            bool result = false;

            RestRequest request = new RestRequest(API_BASE_URL + "user/transfer/" + userId);
            request.AddJsonBody(transfer);
            IRestResponse response = client.Put(request);

            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                throw new Exception("An error occurred communicating with the server.");
            }
            else if (!response.IsSuccessful)
            {
                throw new Exception("An error response was received from the server. The status code is " + (int)response.StatusCode);
            }


            return result;
        }

        public List<TransferMoney> ViewTransfer(int userId)
        {
            RestRequest request = new RestRequest(API_BASE_URL + "user/transferList/" + userId);
            IRestResponse<List<TransferMoney>> response = client.Get<List<TransferMoney>>(request);

            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                throw new Exception("An error occurred communicating with the server.");
            }
            else if (!response.IsSuccessful)
            {
                throw new Exception("An error response was received from the server. The status code is " + (int)response.StatusCode);
            }
            return response.Data;
        }

        public List<TransferMoney> SingleTransfer(int id)
        {
            RestRequest request = new RestRequest(API_BASE_URL + "user/transfer/" + id);
            IRestResponse<List<TransferMoney>> response = client.Get<List<TransferMoney>>(request);

            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                throw new Exception("An error occurred communicating with the server.");
            }
            else if (!response.IsSuccessful)
            {
                throw new Exception("An error response was received from the server. The status code is " + (int)response.StatusCode);
            }
            return response.Data;
        }
    }
}
