using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models.Requests;

public record PutMeasurementRequest(
    string Name,
    string Unit,
    string ApiUrl,
    string FieldJsonPath);
