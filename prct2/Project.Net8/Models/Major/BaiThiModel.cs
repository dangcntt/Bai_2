using DTC.DefaultRepository.Constants;
using DTC.DefaultRepository.Models.Base;
using DTC.T;
using MongoDB.Bson.Serialization.Attributes;
using Project.Net8.Models.Core;
using CoreModel = DTC.DefaultRepository.Models.Core.CoreModel;

namespace Project.Net8.Models.Major;

public class BaiThiModel: Audit, TEntity<string>
{

    public string MoTaNgan  { get; set; }   
    public string NoiDung { get; set; }
    public FileShortModel HinhAnh { get; set; }
    
}
