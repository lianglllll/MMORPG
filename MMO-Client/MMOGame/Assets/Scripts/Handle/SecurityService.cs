using Common.Summer.Core;
using Common.Summer.Net;
using HS.Protobuf.Common;
using HSFramework.MySingleton;
using Serilog;

public class SecurityService : SingletonNonMono<SecurityService>
{
    public bool Init()
    {
        ProtoHelper.Instance.Register<ExchangePublicKeyRequest>((int)CommonProtocl.ExchangePublicKeyReq);
        ProtoHelper.Instance.Register<ExchangePublicKeyResponse>((int)CommonProtocl.ExchangePublicKeyResp);
        ProtoHelper.Instance.Register<ExchangeCommunicationSecretKeyRequest>((int)CommonProtocl.ExchangeCommunicationSecretKeyReq);
        ProtoHelper.Instance.Register<ExchangeCommunicationSecretKeyResponse>((int)CommonProtocl.ExchangeCommunicationSecretKeyResp);

        MessageRouter.Instance.Subscribe<ExchangePublicKeyResponse>(_HandleExchangePublicKeyResponse);
        MessageRouter.Instance.Subscribe<ExchangeCommunicationSecretKeyResponse>(_HandleExchangeCommunicationSecretKeyResponse);

        return true;
    }

    public bool UnInit()
    {
        return true;
    }

    public bool SendExchangePublicKeyRequest()
    {
        ExchangePublicKeyRequest req = new();
        req.ClientPublicKey = NetManager.Instance.curNetClient.EncryptionManager.GetLocalRsaPublicKey(); 
        NetManager.Instance.Send(req);
        return true;
    }
    private void _HandleExchangePublicKeyResponse(Connection sender, ExchangePublicKeyResponse message)
    {
        NetManager.Instance.curNetClient.EncryptionManager.SetRemoteRsaPublicKey(message.ServerPublilcKey);
        SendExchangeCommunicationSecretKeyRequest();
    }
    public bool SendExchangeCommunicationSecretKeyRequest()
    {
        var kv = NetManager.Instance.curNetClient.EncryptionManager.GenerateAesKeyAndIv();
        NetManager.Instance.curNetClient.EncryptionManager.SetAesKeyAndIv(kv.key, kv.iv);
        ExchangeCommunicationSecretKeyRequest req = new();
        // 用对方的rsa公钥加密两个key
        req.Key1 = NetManager.Instance.curNetClient.EncryptionManager.RsaEncrypt(kv.key);
        req.Key2 = NetManager.Instance.curNetClient.EncryptionManager.RsaEncrypt(kv.iv);
        NetManager.Instance.Send(req);
        return true;
    }
    private void _HandleExchangeCommunicationSecretKeyResponse(Connection sender, ExchangeCommunicationSecretKeyResponse message)
    {
        if(message.ResultCode == 0)
        {
            Log.Information("密钥交换完成！！");
        }
    }

}
