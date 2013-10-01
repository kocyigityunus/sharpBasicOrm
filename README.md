sharpBasicOrm
=============

not: this system uses sqlite. you may need to install system.data.sqlite at your visual studio.

using : 

			public class OGRENCI : ModelBase
    		{
        		[MaxLength(50)]
        		public string ADSOYADFIELD;
        		public int NUMARAFIELD;
        		public double NOTORTALAMASIFIELD;
        		public bool MEZUNFIELD;
        		public DateTime OKULABASLAMATARIHIFIELD;
    		}

            DataCore dc = null;

            try
            {
                dc = new DataCore();
                dc.connectAndInitialize();

                OGRENCI ogr = new OGRENCI();
                ogr.MEZUNFIELD = false;
                ogr.NUMARAFIELD = 100202045;
                ogr.NOTORTALAMASIFIELD = 1.5;
                ogr.OKULABASLAMATARIHIFIELD = DateTime.Now;
                ogr.ADSOYADFIELD = "Ogrenci Adi Soyadi";
                ogr.Insert(dc);
                ogr.Dispose();
                
                Console.ReadKey();

                OGRENCI[] ogrenciler = Session.FindObjects<OGRENCI>(dc, "SELECT * FROM OGRENCI");

                OGRENCI yunus = Session.FindObject<OGRENCI>(dc, 1);

                Console.ReadKey();
            }
            catch (Exception myExp)
            {
                throw myExp;
            }
            finally
            {
                dc.disconnectAndDispose();
            }


a basic orm for c#
