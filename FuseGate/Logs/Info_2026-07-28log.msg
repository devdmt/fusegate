module: RequestMpesaExpress  Info:  {"BusinessShortCode":"65102","Password":"NjUxMDI5ZWUxYjdkNThhMDdlMDRkNDA3YmQyY2QzZGE1M2UyNTY3MzdmNjczODRhNTE0MzBmNWEzNTNhMmE2ZjIxNDk0MjAyNjA3MjgxMjEzMDA=","Timestamp":"20260728121300","TransactionType":"CustomerPayBillOnline","Amount":"1","PartyA":"254715315447","PartyB":"65102","PhoneNumber":"254715315447","CallBackURL":"https://8ms93tfs-2041.uks1.devtunnels.ms/processresult/processinsurestkresult","AccountReference":"26-WEPDZ8B","TransactionDesc":"7eddc8d7-d3e4-498f-9cb2-36eef96177ee"}  28/07/2026 12:13:29
module: RequestMpesaExpress  Info:  {
            "MerchantRequestID":"4d4e-495c-83a6-79b21d60ccc818828883",
            "CheckoutRequestID":"ws_CO_28072026121330047715315447",
            "ResponseCode": "0",
            "ResponseDescription":"Success. Request accepted for processing",
            "CustomerMessage":"Success. Request accepted for processing"
        }
          28/07/2026 12:13:29
module: ProcessPensionSTKResult  Info:  test  28/07/2026 13:37:01
module: ProcessPensionSTKResult  Info:  test  28/07/2026 13:38:36
module: ProcessInsureSTKResult  Info:  {"Body":{"stkCallback":{"MerchantRequestID":"68b0-41e7-a936-79b65ab44f7e9106537","CheckoutRequestID":"ws_CO_28072026131735482707148852","ResultCode":1032,"ResultDesc":"Request Cancelled by user."}}}  28/07/2026 13:38:39
module: ProcessPensionSTKResult  Info:  SELECT isnull(Id,0) as trnId,ContributionTrnId as productId,ProcessBatch FROM sTKPushMpesaTransactions where MerchantRequestID= '68b0-41e7-a936-79b65ab44f7e9106537' and CheckoutRequestID ='ws_CO_28072026131735482707148852' and Processed ='0'   28/07/2026 13:38:40
