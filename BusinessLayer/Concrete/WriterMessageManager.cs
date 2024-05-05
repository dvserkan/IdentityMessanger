using BusinessLayer.Abstract;
using DataAccessLayer.Abstract;
using EntityLayer.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Concrete
{
    public class WriterMessageManager : IWriterMessageService
    {
        IWriterMessageDal _writermessage;

        public WriterMessageManager(IWriterMessageDal writermessage)
        {
            _writermessage = writermessage;
        }

        public List<WriterMessage> GetListReceiverMessage(string p)
        {
            return _writermessage.GetbyFilter(x => x.Receiver == p).OrderByDescending(x=> x.Date).ToList();
        }

        public List<WriterMessage> GetListSenderMessage(string p)
        {
            return _writermessage.GetbyFilter(x => x.Sender == p).OrderByDescending(x => x.Date).ToList();
        }

        public void TAdd(WriterMessage t)
        {
            _writermessage.Insert(t);
        }

        public void TDelete(WriterMessage t)
        {
            _writermessage.Delete(t);
        }

        public WriterMessage TGetByID(int id)
        {
            return _writermessage.GetByID(id);
        }

        public List<WriterMessage> TGetlist()
        {
            return _writermessage.GetList();
        }

        public List<WriterMessage> TGetListbyFilter()
        {
            throw new NotImplementedException();
        }

        public void TUpdate(WriterMessage t)
        {
            _writermessage.Update(t);
        }
    }
}
