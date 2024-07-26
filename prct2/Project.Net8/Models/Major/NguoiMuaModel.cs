using DTC.DefaultRepository.Constants;
using DTC.DefaultRepository.Models.Base;
using DTC.T;
using MongoDB.Bson.Serialization.Attributes;

namespace Project.Net8.Models.Major;

public class NguoiMuaModel: Audit, TEntity<string>
{
    
    public string SoCMND { get; set; }

    public DateTime? NgayKyHopDong { get; set; }
    
    [BsonIgnore]
    public string PublicationDateShow
    {
        get { return NgayKyHopDong.HasValue ? NgayKyHopDong.Value.ToLocalTime().ToString(FormatTime.FORMAT_DATE_CORE) : ""; }
    }    
    public string SDT { get; set; }
    

}
