using AdresApi.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace AdresApi.Services
{
    public class Datab:IDatab
    {
        NumberFormatInfo nfi = (NumberFormatInfo)CultureInfo.CurrentCulture.NumberFormat.Clone();
        const string DBFILE = "rra.dat";
        //Data Table Structure:
        //0)id,1)presupuesto,2)unidad,3)tipo,4)cantidad,5)valorUnitario,6)valorTotal,7)fecha,8)proveedor,9)documentacion

        public DefaultResponse UpdateRecord(RRRAData Data)
        {
            var _res = new DefaultResponse();
            try
            {
                int nLineNumber=SearchId(Data.id);
                if (nLineNumber <= 0)
                    throw new Exception("No existe el registro");
                else
                {
                    if (DeleteLine(nLineNumber))
                    {
                        _res=AddRecord(Data);
                    }
                    else
                        throw new Exception("Error al eliminar registro");
                }
            }
            catch (Exception ex)
            {
                _res.isValid = false; _res.stringValue = ex.Message;
            }
            return _res;
        }

        public DefaultResponse DeleteRecord(double id)
        {
            var _res = new DefaultResponse();
            try
            {
                int nLineNumber = SearchId(id);
                if (nLineNumber<= 0)
                    throw new Exception("No existe el registro");
                else
                {
                    if (DeleteLine(nLineNumber))
                    {
                        _res.isValid = true; _res.numberValue = 1;
                        _res.stringValue = "Registro eliminado";
                    }
                    else
                        throw new Exception("Error al eliminar registro");
                }
            }
            catch (Exception ex)
            {
                _res.isValid = false; _res.stringValue = ex.Message;
            }
            return _res;
        }

        public RData GetData(double id)
        {
            var _res = new RData();
            try
            {
                using (StreamReader sr = new StreamReader(DBFILE))
                {
                    string line;
                    // Leer cada línea del archivo
                    int nRegs = 0;
                    while ((line = sr.ReadLine()) != null)
                    {
                        string[] Fields = line.Split('|');
                        if (Fields[0].Trim()==id.ToString().Trim())
                        {
                            var _Item= new RRRAData
                            {
                                id = StToIn(Fields[0]),
                                presupuesto = StToDb(Fields[1]),
                                unidad = Fields[2],
                                tipo = Fields[3],
                                cantidad = StToDb(Fields[4]),
                                valorUnitario = StToDb(Fields[5]),
                                valorTotal = StToDb(Fields[6]),
                                fecha = StToDt(Fields[7]),
                                proveedor = Fields[8],
                                documentacion = Fields[9]
                            };
                            nRegs++;
                            _res.isValid = true; _res.numberValue = nRegs;
                            _res.stringValue = "Registro encontrado"; _res.resultData = _Item;
                            break;
                        }
                    }
                    if (nRegs==0)
                    throw new Exception("No existe el registro");
                }
            }
            catch (Exception ex)
            {
                _res.isValid = false; _res.stringValue = ex.Message;
            }
            return _res;
        }

        public DefaultResponse AddRecord(RRAData Data)
        {
            var _res= new DefaultResponse();    
            try { 
            string _data=CreateTextRecord(Data);
                if (string.IsNullOrEmpty(_data))
                    throw new Exception("Error en formato de datos");
                File.AppendAllText(DBFILE, _data + Environment.NewLine);
                _res.isValid = true; _res.stringValue = "Registro creado";
            } 
                catch (Exception ex) 
            {
                _res.isValid=false; _res.stringValue = ex.Message;
            }
            return _res;
        }

        public RFilterData FilterData(FilterData Data)
        {
            var _res = new RFilterData();
            try
            {
                var _Items = new List<RRRAData>();
                using (StreamReader sr = new StreamReader(DBFILE))
                {
                    string line;
                    // Leer cada línea del archivo
                    int nRegs = 0;
                    while ((line = sr.ReadLine()) != null)
                    {
                        string[] Fields = line.Split('|');
                        if ((Data.unidad == string.Empty || Fields[2].Contains(Data.unidad.ToUpper())) &&
                           (Data.tipo == string.Empty || Fields[3].Contains(Data.tipo.ToUpper())) &&
                           (Data.fecha.Year == 2000 || Fields[7].Contains(Data.fecha.ToString().Substring(0,10))) &&
                           (Data.proveedor == string.Empty || Fields[8].Contains(Data.unidad.ToUpper())))
                        {
                            _Items.Add(new RRRAData
                            {
                                id = StToIn(Fields[0]),
                                presupuesto = StToDb(Fields[1]),
                                unidad = Fields[2],
                                tipo = Fields[3],
                                cantidad = StToDb(Fields[4]),
                                valorUnitario = StToDb(Fields[5]),
                                valorTotal = StToDb(Fields[6]),
                                fecha = StToDt(Fields[7]),
                                proveedor = Fields[8],
                                documentacion = Fields[9]
                            });
                            nRegs++;
                        }
                    }
                    if (nRegs > 0)
                    {
                        _res.isValid = true; _res.numberValue = nRegs;
                        _res.stringValue = $"{nRegs} Registros"; _res.resultData = _Items;
                    }
                    else throw new Exception("No hay registros");
                }
            }
            catch (Exception ex)
            {
                _res.isValid = false; _res.stringValue = ex.Message;
            }
            return _res;
        }

        private int SearchId(double id)
        {
            int nRegs = 0, nLineNumber = 0;
            string line;
            try
            {
                using (StreamReader sr = new StreamReader(DBFILE))
                {
                    // Leer cada línea del archivo
                    while ((line = sr.ReadLine()) != null)
                    {
                        string[] Fields = line.Split('|');
                        if (Fields[0].Trim() == id.ToString().Trim())
                        {
                            nRegs++;
                            break;
                        }
                        nLineNumber++;
                    }
                }
                if (nRegs == 0)
                    nLineNumber = 0;
            }
            catch
            {
                nLineNumber = -1;
            }
            return nLineNumber;
        }

        private bool DeleteLine(int LineNumber)
        {
            bool _Res = true;
            try
            {
                List<string> lineas = new List<string>(File.ReadAllLines(DBFILE));
                lineas.RemoveAt(LineNumber);
                File.WriteAllLines(DBFILE, lineas);
            }
            catch
            {
                _Res = false;
            }
            return _Res;
        }

        private int RandomValue()
        {
            int _Limiteinferior, _Limitesuperior;
                _Limiteinferior = 10 ^ (5 - 1);
                _Limitesuperior = Convert.ToInt32("9".PadLeft(5, '9'));
            Random random = new Random();
            int RandomNumber = random.Next(_Limiteinferior, _Limitesuperior);
            return RandomNumber;
        }

        private string DbToSt<T>(T PValue)
        {
            nfi.NumberDecimalSeparator = ".";
            nfi.CurrencyDecimalSeparator = ".";
            return Convert.ToDouble(PValue).ToString("G", nfi);
        }

        private int StToIn<T>(T PValue)
        {
            return Convert.ToInt32(PValue);
        }
        private double StToDb<T>(T PValue)
        {
            return Convert.ToDouble(PValue);
        }

        private DateTime StToDt<T>(T PValue)
        {
            return Convert.ToDateTime(PValue);
        }

        private string StFrm(string PValue)
        {
            return PValue.Trim().ToUpper();
        }

        private string CreateTextRecord(RRAData Data)
        {
            string _Res = "";
            try
            {
                string sId = RandomValue().ToString();
                _Res = $"{sId}|{DbToSt(Data.presupuesto)}|{StFrm(Data.unidad)}|{StFrm(Data.tipo)}|{DbToSt(Data.cantidad)}|" +
                    $"{DbToSt(Data.valorUnitario)}|{DbToSt(Data.valorTotal)}|{Data.fecha.ToString().Substring(0, 10)}|" +
                    $"{StFrm(Data.proveedor)}|{Data.documentacion}";
            }
            catch { }
            return _Res;
        }

    }
}
