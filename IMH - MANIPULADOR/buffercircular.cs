using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IMH___MANIPULADOR
{
    // BUFFER CIRCULAR
    public class buffercircular
    {
        private Int16 sizeBuffer;
        private Int16[] buffer;
        private Int16 wr, rd;
        public Int16 cont;

        public buffercircular(Int16 _sizeBuffer)
        {
            sizeBuffer = _sizeBuffer;
            buffer = new Int16[_sizeBuffer];
            wr = 0;
            rd = 0;
            cont = 0;
        }

        public Int16 Count()
        {
            return cont;
        }

        public bool write(Int16 valor)
        {
            if (cont < sizeBuffer)
            {
                buffer[wr] = valor;
                cont++;
                wr++;

                if (wr >= sizeBuffer)
                {
                    wr = 0;
                }
                return true;
            }

            else
            {
                MessageBox.Show("Estouro do Buffer", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public Int16 read()
        {
            if (cont == 0)
            {
                return 9876; // Valor irreal para dizer que não é preciso plotar
            }

            else
            {
                Int16 val = buffer[rd];
                cont--;
                rd++;
                if (rd >= sizeBuffer)
                {
                    rd = 0;
                }
                return val;
            }
        }
    }






}
