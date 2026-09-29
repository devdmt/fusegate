module: ProcessPensionSTKResult  Info:  test  04/05/2026 10:51:40
module: ProcessPensionSTKResult  Info:  test  04/05/2026 10:52:14
module: ProcessPensionSTKResult  Info:  {"Body":{"stkCallback":{"MerchantRequestID":"4ddd-4866-824e-3f944c695ba349436671","CheckoutRequestID":"ws_CO_04052026103525783715315447","ResultCode":0,"ResultDesc":"The service request is processed successfully.","CallbackMetadata":{"Item":[{"Name":"Amount","Value":1},{"Name":"MpesaReceiptNumber","Value":"UE4DZ32LQP"},{"Name":"Balance"},{"Name":"TransactionDate","Value":20260504103534},{"Name":"PhoneNumber","Value":254715315447}]}}}}   04/05/2026 10:52:17
module: ProcessPensionSTKResult  Info:  SELECT isnull(Id,0) as trnId,ContributionTrnId as productId,ProcessBatch FROM sTKPushMpesaTransactions where MerchantRequestID= '4ddd-4866-824e-3f944c695ba349436671' and CheckoutRequestID ='ws_CO_04052026103525783715315447' and Processed ='0'   04/05/2026 10:52:25
module: AuthenticatePartner  Info:  No  04/05/2026 14:40:15
module: AuthenticatePartner  Info:  No  04/05/2026 14:41:21
module: transfer  Info:  partner LATBS9Y --pcode   04/05/2026 14:50:19
module: transfer  Info:  partner LATBS9Y --pcode LATBS9Y}  04/05/2026 14:51:05
module: RequestMpesaExpress  Info:  {"BusinessShortCode":"65102","Password":"NjUxMDI5ZWUxYjdkNThhMDdlMDRkNDA3YmQyY2QzZGE1M2UyNTY3MzdmNjczODRhNTE0MzBmNWEzNTNhMmE2ZjIxNDk0MjAyNjA1MDQwMzE1MTk=","Timestamp":"20260504031519","TransactionType":"CustomerPayBillOnline","Amount":"1","PartyA":"254715315447","PartyB":"65102","PhoneNumber":"254715315447","CallBackURL":"https://api.sanlam.co.ke/uatncbampesacallback","AccountReference":"25-KPUQ8C","TransactionDesc":"8d3c3afc-e8df-451a-6c44-08dea9d6cca7"}  04/05/2026 15:15:19
module: RequestMpesaExpress  Info:  {
            "MerchantRequestID":"003b-44b6-9756-96f607929e892858606",
            "CheckoutRequestID":"ws_CO_04052026151520088715315447",
            "ResponseCode": "0",
            "ResponseDescription":"Success. Request accepted for processing",
            "CustomerMessage":"Success. Request accepted for processing"
        }
          04/05/2026 15:15:20
module: RequestMpesaExpress  Info:  {"BusinessShortCode":"65102","Password":"NjUxMDI5ZWUxYjdkNThhMDdlMDRkNDA3YmQyY2QzZGE1M2UyNTY3MzdmNjczODRhNTE0MzBmNWEzNTNhMmE2ZjIxNDk0MjAyNjA1MDQwMzE4MDc=","Timestamp":"20260504031807","TransactionType":"CustomerPayBillOnline","Amount":"1","PartyA":"254715315447","PartyB":"65102","PhoneNumber":"254715315447","CallBackURL":"https://api.sanlam.co.ke/uatncbampesacallback","AccountReference":"25-KPUQ8C","TransactionDesc":"d2b8de11-ced3-485f-d765-08dea9d7318a"}  04/05/2026 15:18:08
module: RequestMpesaExpress  Info:  {
            "MerchantRequestID":"435b-4e2a-b040-249cf8f3669f1088016",
            "CheckoutRequestID":"ws_CO_04052026151809337715315447",
            "ResponseCode": "0",
            "ResponseDescription":"Success. Request accepted for processing",
            "CustomerMessage":"Success. Request accepted for processing"
        }
          04/05/2026 15:18:09
module: RequestMpesaExpress  Info:  {"BusinessShortCode":"65102","Password":"NjUxMDI5ZWUxYjdkNThhMDdlMDRkNDA3YmQyY2QzZGE1M2UyNTY3MzdmNjczODRhNTE0MzBmNWEzNTNhMmE2ZjIxNDk0MjAyNjA1MDQwMzIyMzg=","Timestamp":"20260504032238","TransactionType":"CustomerPayBillOnline","Amount":"1","PartyA":"254715315447","PartyB":"65102","PhoneNumber":"254715315447","CallBackURL":"https://api.sanlam.co.ke/uatncbampesacallback","AccountReference":"25-KPUQ8C","TransactionDesc":"d946cdab-fd78-491a-d766-08dea9d7318a"}  04/05/2026 15:22:38
module: RequestMpesaExpress  Info:  {
            "MerchantRequestID":"fb7c-4eba-ba7b-32a1ea319bc52657601",
            "CheckoutRequestID":"ws_CO_04052026152239864715315447",
            "ResponseCode": "0",
            "ResponseDescription":"Success. Request accepted for processing",
            "CustomerMessage":"Success. Request accepted for processing"
        }
          04/05/2026 15:22:39
