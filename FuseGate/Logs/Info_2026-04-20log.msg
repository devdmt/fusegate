module: RequestMpesaExpress  Info:  {"BusinessShortCode":"65102","Password":"NjUxMDI5ZWUxYjdkNThhMDdlMDRkNDA3YmQyY2QzZGE1M2UyNTY3MzdmNjczODRhNTE0MzBmNWEzNTNhMmE2ZjIxNDk0MjAyNjA0MjAxMDM1Mzk=","Timestamp":"20260420103539","TransactionType":"CustomerPayBillOnline","Amount":"100","PartyA":"254715315447","PartyB":"65102","PhoneNumber":"254715315447","CallBackURL":"https://api.sanlam.co.ke/uatncbampesacallback","AccountReference":"26-AFLMM3Q","TransactionDesc":"8834d465-4d9e-4a66-ac88-b52ea2e23273"}  20/04/2026 10:35:39
module: RequestMpesaExpress  Info:  {
            "MerchantRequestID":"6ef1-443b-b41c-b48219bb5f936891771",
            "CheckoutRequestID":"ws_CO_20042026103539781715315447",
            "ResponseCode": "0",
            "ResponseDescription":"Success. Request accepted for processing",
            "CustomerMessage":"Success. Request accepted for processing"
        }
          20/04/2026 10:35:40
module: ProcessPensionSTKResult  Info:  test  20/04/2026 10:44:19
module: ProcessPensionSTKResult  Info:  test  20/04/2026 12:03:03
module: RequestMpesaExpress  Info:  {"BusinessShortCode":"65102","Password":"NjUxMDI5ZWUxYjdkNThhMDdlMDRkNDA3YmQyY2QzZGE1M2UyNTY3MzdmNjczODRhNTE0MzBmNWEzNTNhMmE2ZjIxNDk0MjAyNjA0MjAwMjA3MzE=","Timestamp":"20260420020731","TransactionType":"CustomerPayBillOnline","Amount":"22","PartyA":"254715315447","PartyB":"65102","PhoneNumber":"254715315447","CallBackURL":"https://8ms93tfs-2041.uks1.devtunnels.ms/processresult/processinsurestkresult","AccountReference":"26-AFLMM3Q","TransactionDesc":"2f91629d-05f3-4b1a-bf9b-ee117555aace"}  20/04/2026 14:07:32
module: RequestMpesaExpress  Info:  {
            "MerchantRequestID":"e356-46bc-981d-c7eb48e9e49c3276422",
            "CheckoutRequestID":"ws_CO_20042026140732460715315447",
            "ResponseCode": "0",
            "ResponseDescription":"Success. Request accepted for processing",
            "CustomerMessage":"Success. Request accepted for processing"
        }
          20/04/2026 14:07:32
module: ProcessPensionSTKResult  Info:  test  20/04/2026 14:08:08
module: ProcessInsureSTKResult  Info:  {"Body":{"stkCallback":{"MerchantRequestID":"e356-46bc-981d-c7eb48e9e49c3276422","CheckoutRequestID":"ws_CO_20042026140732460715315447","ResultCode":1037,"ResultDesc":"No response from user."}}}  20/04/2026 14:08:39
module: ProcessPensionSTKResult  Info:  test  20/04/2026 14:08:44
module: ProcessInsureSTKResult  Info:  {"Body":{"stkCallback":{"MerchantRequestID":"e356-46bc-981d-c7eb48e9e49c3276422","CheckoutRequestID":"ws_CO_20042026140732460715315447","ResultCode":1037,"ResultDesc":"No response from user."}}}  20/04/2026 14:08:48
module: ProcessPensionSTKResult  Info:  SELECT isnull(Id,0) as trnId,ContributionTrnId as productId,ProcessBatch FROM sTKPushMpesaTransactions where MerchantRequestID= 'e356-46bc-981d-c7eb48e9e49c3276422' and CheckoutRequestID ='ws_CO_20042026140732460715315447' and Processed ='0'   20/04/2026 14:08:50
module: ProcessPensionSTKResult  Info:  SELECT isnull(Id,0) as trnId,ContributionTrnId as productId,ProcessBatch FROM sTKPushMpesaTransactions where MerchantRequestID= 'e356-46bc-981d-c7eb48e9e49c3276422' and CheckoutRequestID ='ws_CO_20042026140732460715315447' and Processed ='0'   20/04/2026 14:08:55
module: RequestMpesaExpress  Info:  {"BusinessShortCode":"65102","Password":"NjUxMDI5ZWUxYjdkNThhMDdlMDRkNDA3YmQyY2QzZGE1M2UyNTY3MzdmNjczODRhNTE0MzBmNWEzNTNhMmE2ZjIxNDk0MjAyNjA0MjAwMjE1MTY=","Timestamp":"20260420021516","TransactionType":"CustomerPayBillOnline","Amount":"22","PartyA":"254715315447","PartyB":"65102","PhoneNumber":"254715315447","CallBackURL":"https://8ms93tfs-2041.uks1.devtunnels.ms/processresult/processinsurestkresult","AccountReference":"26-AFLMM3Q","TransactionDesc":"e90fd5e0-c0d9-4c2d-a343-cda932bdb24e"}  20/04/2026 14:15:17
module: RequestMpesaExpress  Info:  {
            "MerchantRequestID":"2808-4281-b9ee-c47c41722f75762753",
            "CheckoutRequestID":"ws_CO_20042026141522334715315447",
            "ResponseCode": "0",
            "ResponseDescription":"Success. Request accepted for processing",
            "CustomerMessage":"Success. Request accepted for processing"
        }
          20/04/2026 14:15:22
module: ProcessPensionSTKResult  Info:  test  20/04/2026 14:16:14
module: ProcessPensionSTKResult  Info:  test  20/04/2026 14:16:14
module: RequestMpesaExpress  Info:  {"BusinessShortCode":"65102","Password":"NjUxMDI5ZWUxYjdkNThhMDdlMDRkNDA3YmQyY2QzZGE1M2UyNTY3MzdmNjczODRhNTE0MzBmNWEzNTNhMmE2ZjIxNDk0MjAyNjA0MjAwMjE3MDc=","Timestamp":"20260420021707","TransactionType":"CustomerPayBillOnline","Amount":"7","PartyA":"254715315447","PartyB":"65102","PhoneNumber":"254715315447","CallBackURL":"https://8ms93tfs-2041.uks1.devtunnels.ms/processresult/processinsurestkresult","AccountReference":"26-AFLMM3Q","TransactionDesc":"1de2b95b-b11a-4ab9-b47c-84da15e05462"}  20/04/2026 14:17:07
module: RequestMpesaExpress  Info:  {
            "MerchantRequestID":"2808-4281-b9ee-c47c41722f75765064",
            "CheckoutRequestID":"ws_CO_20042026141708371715315447",
            "ResponseCode": "0",
            "ResponseDescription":"Success. Request accepted for processing",
            "CustomerMessage":"Success. Request accepted for processing"
        }
          20/04/2026 14:17:08
module: ProcessPensionSTKResult  Info:  test  20/04/2026 14:17:15
module: ProcessInsureSTKResult  Info:  {"Body":{"stkCallback":{"MerchantRequestID":"2808-4281-b9ee-c47c41722f75765064","CheckoutRequestID":"ws_CO_20042026141708371715315447","ResultCode":1037,"ResultDesc":"DS timeout user cannot be reached."}}}  20/04/2026 14:17:15
module: ProcessPensionSTKResult  Info:  SELECT isnull(Id,0) as trnId,ContributionTrnId as productId,ProcessBatch FROM sTKPushMpesaTransactions where MerchantRequestID= '2808-4281-b9ee-c47c41722f75765064' and CheckoutRequestID ='ws_CO_20042026141708371715315447' and Processed ='0'   20/04/2026 14:17:15
module: ProcessPensionSTKResult  Info:  test  20/04/2026 14:17:24
module: ProcessInsureSTKResult  Info:  {"Body":{"stkCallback":{"MerchantRequestID":"2808-4281-b9ee-c47c41722f75765064","CheckoutRequestID":"ws_CO_20042026141708371715315447","ResultCode":1037,"ResultDesc":"DS timeout user cannot be reached."}}}  20/04/2026 14:17:27
module: ProcessPensionSTKResult  Info:  test  20/04/2026 14:17:33
module: ProcessInsureSTKResult  Info:  {"Body":{"stkCallback":{"MerchantRequestID":"2808-4281-b9ee-c47c41722f75765064","CheckoutRequestID":"ws_CO_20042026141708371715315447","ResultCode":1037,"ResultDesc":"DS timeout user cannot be reached."}}}  20/04/2026 14:17:33
module: ProcessPensionSTKResult  Info:  SELECT isnull(Id,0) as trnId,ContributionTrnId as productId,ProcessBatch FROM sTKPushMpesaTransactions where MerchantRequestID= '2808-4281-b9ee-c47c41722f75765064' and CheckoutRequestID ='ws_CO_20042026141708371715315447' and Processed ='0'   20/04/2026 14:17:33
module: ProcessPensionSTKResult  Info:  SELECT isnull(Id,0) as trnId,ContributionTrnId as productId,ProcessBatch FROM sTKPushMpesaTransactions where MerchantRequestID= '2808-4281-b9ee-c47c41722f75765064' and CheckoutRequestID ='ws_CO_20042026141708371715315447' and Processed ='0'   20/04/2026 14:17:48
module: ProcessPensionSTKResult  Info:  test  20/04/2026 14:30:35
module: ProcessPensionSTKResult  Info:  test  20/04/2026 14:31:46
module: ProcessInsureSTKResult  Info:  {
    "Body": {
        "stkCallback": {
            "MerchantRequestID": "2808-4281-b9ee-c47c41722f75765064",
            "CheckoutRequestID": "ws_CO_20042026141708371715315447",
            "ResultCode": 1037,
            "ResultDesc": "DS timeout user cannot be reached."
        }
    }
}  20/04/2026 14:31:54
module: ProcessPensionSTKResult  Info:  SELECT isnull(Id,0) as trnId,ContributionTrnId as productId,ProcessBatch FROM sTKPushMpesaTransactions where MerchantRequestID= '2808-4281-b9ee-c47c41722f75765064' and CheckoutRequestID ='ws_CO_20042026141708371715315447' and Processed ='0'   20/04/2026 14:32:03
module: ProcessPensionSTKResult  Info:  test  20/04/2026 14:33:23
module: ProcessInsureSTKResult  Info:  {
    "Body": {
        "stkCallback": {
            "MerchantRequestID": "2808-4281-b9ee-c47c41722f75765064",
            "CheckoutRequestID": "ws_CO_20042026141708371715315447",
            "ResultCode": 1037,
            "ResultDesc": "DS timeout user cannot be reached."
        }
    }
}  20/04/2026 14:33:29
module: ProcessPensionSTKResult  Info:  SELECT isnull(Id,0) as trnId,ContributionTrnId as productId,ProcessBatch FROM sTKPushMpesaTransactions where MerchantRequestID= '2808-4281-b9ee-c47c41722f75765064' and CheckoutRequestID ='ws_CO_20042026141708371715315447' and Processed ='0'   20/04/2026 14:33:33
