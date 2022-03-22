using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IMH___MANIPULADOR
{
    public class TratamentoDados //: Form
    {
        private const int etapas = 13;
        private const int alcances = 10;

        Button btn_inicia = new Button();
        // Variaveis gerais da classe
        public Mutex mut;
        private SerialPort serialPort1;
        private Thread ThrAquis;
        private buffercircular BFFCplot, BFFCGrav; // Buffer Circular para plotagem e gravação
                                                   //private Form1 classMain; // Buffer Circular para plotagem
                                                   //private int LSB, MSB;
                                                   //private Int16 num;
        public bool run;//, Finish1, Finish2, Finish3;
        public short FatorMov;
        private const int SizeBuffer = 10000; // tamanho do vetor do buffer circular
        private bool flag_buffer;
        private byte[] Dados;
        private Int16 CheckSum;

        // Gravar dados
        private System.IO.TextWriter dadotxt;
        OpenFileDialog openFileDialog1;
        private String Nome_arquivo, Local_arquivo;
        private Int16 Posicao1 = 0;
        private Int16 Posicao2 = 0;
        private Int16 Corrente1 = 0;
        private Int16 Corrente2 = 0;

        private Panel pbMovArea = new Panel();
        private PictureBox pbManopla = new PictureBox();
        private PictureBox pbAlvo = new PictureBox();
        private Panel pbAcerto = new Panel();

        public int x, y;
        // teste lbl
        private Label lbl1, lbl2, lbl3, lbl4;

        // Pistas visuais
        // 110 valores entre 1 e 0 (52 em 1)
        public int[] randomSide1 = new int[110] { 1, 1, 1, 0, 0, 0, 1, 0, 0, 1, 1, 1, 1, 0, 1, 0, 0, 1, 0, 0, 0, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 1, 1, 0, 0, 1, 1, 0, 1, 0, 0, 1, 1, 1, 1, 1, 0, 1, 0, 0, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 1, 1, 0, 1, 0, 1, 0, 0, 0, 0, 0, 1, 0, 1, 1, 0, 0, 0, 0, 0, 1, 0, 1, 0, 1, 0, 1, 1, 1 };
        // 110 valores entre 1 e 0 (50 em 1)
        public int[] randomSide2 = new int[110] { 1, 0, 1, 1, 1, 0, 0, 1, 1, 0, 1, 1, 0, 1, 1, 1, 0, 0, 1, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 1, 0, 1, 1, 1, 0, 0, 1, 1, 0, 0, 1, 1, 0, 0, 0, 0, 1, 1, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 1, 0, 1, 0, 1, 0, 0, 1, 0, 1, 0, 1, 1, 1, 0, 1, 0, 1, 0, 0, 1, 0, 1, 1, 0, 0, 1, 1, 0, 1, 0, 0, 0, 1, 0, 0, 1, 1, 0, 1, 1, 1, 0, 0, 1 };
        // 110 valores entre 1 e 0 (55 em 1)
        public int[] randomSide3 = new int[110] { 1, 0, 0, 1, 0, 1, 1, 1, 1, 1, 0, 0, 1, 0, 1, 0, 1, 0, 0, 1, 0, 0, 0, 1, 0, 1, 1, 0, 1, 1, 0, 0, 1, 1, 1, 0, 0, 0, 1, 0, 0, 1, 0, 1, 1, 0, 1, 0, 0, 1, 1, 0, 0, 0, 1, 0, 1, 0, 1, 0, 1, 1, 0, 1, 1, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 0, 0, 1, 0, 1, 0, 1, 0, 0, 1, 0, 0, 1, 1, 1, 1, 1, 0, 1, 1, 1, 0, 0, 1, 0, 0, 0, 1, 1, 0, 1, 0, 0 };
        // 110 valores entre 1 e 0 (53 em 1)
        public int[] randomSide4 = new int[110] { 1, 0, 1, 0, 1, 1, 0, 1, 1, 1, 1, 1, 1, 0, 0, 1, 1, 1, 1, 0, 0, 0, 1, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 1, 0, 1, 1, 1, 1, 1, 0, 0, 1, 1, 0, 1, 0, 0, 0, 0, 0, 1, 0, 0, 1, 0, 1, 1, 0, 1, 0, 1, 1, 1, 0, 0, 0, 0, 1, 0, 0, 1, 0, 0, 0, 1, 1, 0, 1, 0, 0, 0, 1, 1, 0, 1, 1, 1, 0, 0, 0, 1, 1, 0, 0, 1, 1, 0, 1, 0, 1, 1 };

        public Byte flag_pistaVisual;
        Pen myPen = new Pen(Color.BlueViolet, 1);
        Graphics g = null;
        static int start_x = 0, start_y = 0;
        static int end_x = 0, end_y = 0;

        // Alvo na tela
        Pen alvoPen = new Pen(Color.Gold, 3);
        Graphics a = null;

        public TratamentoDados(SerialPort _serialPort1, Button _btn_inicia, System.IO.TextWriter dadotxt, Thread _ThrAquis, Panel _pbMovArea, PictureBox _pbObjeto, PictureBox _pbPainel, Panel _pbAcertoAlvo, Label _lbl1, Label _lbl2, Label _lbl3, Label _lbl4)
        {
            btn_inicia = _btn_inicia;
            openFileDialog1 = new OpenFileDialog();

            BFFCplot = new buffercircular(SizeBuffer);
            flag_buffer = true;
            //BFFCGrav = new buffercircular(SizeBuffer);
            serialPort1 = _serialPort1;
            ThrAquis = _ThrAquis;
            Dados = new byte[10];
            pbManopla = _pbObjeto;
            pbAlvo = _pbPainel;
            pbAcerto = _pbAcertoAlvo;
            pbMovArea = _pbMovArea;
            FatorMov = 1;
            pbAlvo.Location = new Point(Convert.ToInt16(pbMovArea.Width / 2), Convert.ToInt16(pbMovArea.Height * 0.1));
            pbManopla.Location = new Point(0, Convert.ToInt16(pbMovArea.Height)); // Começando na posição 0,0
            run = false;
            flag_pistaVisual = 0;
            //Finish1 = false;
            //Finish2 = false;
            //Finish2 = false;

            //teste lbl
            lbl1 = _lbl1;
            lbl2 = _lbl2;
            lbl3 = _lbl3;
            lbl4 = _lbl4;
        }

        public void StateMutex(bool _State) // Cancela Mutex
        {
            if (!_State)
            {
                mut.WaitOne();
                mut.ReleaseMutex();
                mut.Dispose();
                mut.Close();
            }
            else
                mut = new Mutex();
        }

        public void Aquisicao()
        {
            while (run)
            {
                mut.WaitOne(); // Comando para aguardar conincidencia de Thread
                while (serialPort1.BytesToRead > 0) // Observa quando o buffer do USB esta com 100 bytes
                {
                    for (Int16 i = 0; i < serialPort1.BytesToRead; i++) // Pega em pares os bytes e coloca no buffer circuilar
                    {
                        byte initD = Convert.ToByte(serialPort1.ReadByte());
                        if (initD == 0x7E) // Confere se o primeiro dado é 0x7e
                        {
                            CheckSum = 0;
                            for (Int16 j = 0; j < 10; j++) // Pega os proximos 9 dados
                            {
                                i++; // Adiciona os dados 
                                while (serialPort1.BytesToRead < 1) ; // Aguarda dado
                                Dados[j] = Convert.ToByte(serialPort1.ReadByte());
                            }
                            if (Dados[9] == 0x81) // Observa se o ultimo dado é 0x81
                            {
                                for (Int16 l = 0; l < 8; l++) // Somatória do Checksun
                                {
                                    CheckSum += Dados[l];
                                }
                                if (Dados[8] == Convert.ToByte(CheckSum & 0xFF)) // Pega o valor lsb ) // Compara com o mandado
                                {
                                    /*
                                    Int16 x = Convert.ToInt16(Dados[0] * 255 + Dados[1]);
                                    Int16 y = Convert.ToInt16(Dados[2] * 255 + Dados[3]);
                                    Int16 w = Convert.ToInt16(Dados[4] * 255 + Dados[5]);
                                    Int16 z = Convert.ToInt16(Dados[6] * 255 + Dados[7]);

                                    BFFCplot.write(Convert.ToInt16(Dados[0] * 256 + Dados[1]));
                                    BFFCplot.write(Convert.ToInt16(Dados[2] * 256 + Dados[3]));
                                    BFFCplot.write(Convert.ToInt16(Dados[4] * 256 + Dados[5]));
                                    BFFCplot.write(Convert.ToInt16(Dados[6] * 256 + Dados[7]));

                                    BFFCGrav.write(Convert.ToInt16(Dados[0] * 256 + Dados[1]));
                                    BFFCGrav.write(Convert.ToInt16(Dados[2] * 256 + Dados[3]));
                                    BFFCGrav.write(Convert.ToInt16(Dados[4] * 256 + Dados[5]));
                                    BFFCGrav.write(Convert.ToInt16(Dados[6] * 256 + Dados[7]));
                                    */

                                    if (flag_buffer)
                                    {
                                        BFFCplot.write(Convert.ToInt16((Dados[0] << 8) + Dados[1]));
                                        BFFCplot.write(Convert.ToInt16((Dados[2] << 8) + Dados[3]));
                                        BFFCplot.write(Convert.ToInt16((Dados[4] << 8) + Dados[5]));
                                        BFFCplot.write(Convert.ToInt16((Dados[6] << 8) + Dados[7]));

                                    
                                        BFFCGrav.write(Convert.ToInt16((Dados[0] << 8) + Dados[1]));
                                        BFFCGrav.write(Convert.ToInt16((Dados[2] << 8) + Dados[3]));
                                        BFFCGrav.write(Convert.ToInt16((Dados[4] << 8) + Dados[5]));
                                        BFFCGrav.write(Convert.ToInt16((Dados[6] << 8) + Dados[7]));
                                    }
                                }
                            }
                        }
                    }
                }
                mut.ReleaseMutex();// Libera outra Thread 
            }
            //Finish1 = true;
        }

        public void LocalArquivos(String _Nome_arquivo, String _Local_arquivo, System.IO.TextWriter _dadotxt)
        {
            BFFCGrav = new buffercircular(SizeBuffer);
            Nome_arquivo = _Nome_arquivo;
            Local_arquivo = _Local_arquivo;

            openFileDialog1.InitialDirectory = Local_arquivo; // Devolve onde está o arquivo salvo
            openFileDialog1.FileName = Nome_arquivo;

            dadotxt = _dadotxt; // Abre o local da pasta
            dadotxt.WriteLine("Pista Visual: " + Convert.ToString(flag_pistaVisual));
            //dadotxt = StreamWriter(Nome_arquivo); // Abre o local da pasta
            //dadotxt.Invoke(new Action(() => dadotxt = System.IO.File.AppendText(Nome_arquivo)));
            //this.Invoke(new Action(() => dadotxt = System.IO.File.AppendText(Nome_arquivo)));
            //dadotxt = System.IO.File.AppendText(Nome_arquivo);
        }
        public void MovimentoN(int _n)
        {
            dadotxt.WriteLine(""); // Escrevendo nova linha de dado
            dadotxt.WriteLine("Movimento " + Convert.ToString(_n));
            dadotxt.WriteLine("");
        }
        public void FimEtapaN(int _n)
        {
            flag_buffer = false;
            dadotxt.WriteLine(""); // Escrevendo nova linha de dado
            dadotxt.WriteLine("Etapa " + Convert.ToString(_n));
            dadotxt.WriteLine("");
            var dataByte = new byte[] { 0x7E, 0x69 };
            serialPort1.Write(dataByte, 0, 2);
            DialogResult dialogResult = MessageBox.Show("Bloco " + Convert.ToString(_n) + " Finalizado!" + "\n" + "Leve o manipulador até a sua ESQUERDA INFERIOR!", "Fim da Etapa", MessageBoxButtons.OK);
            if (dialogResult == System.Windows.Forms.DialogResult.OK)
            {           
                flag_buffer = true;
            }
        }
        public void FinalizaColeta()
        {
            //mut.WaitOne();
            
            run = false;
            //mut.ReleaseMutex(); // Libera outra Thread
            StateMutex(false);
            var dataByte = new byte[] { 0x7E, 0x10};
            serialPort1.Write(dataByte, 0, 2);
            btn_inicia.Invoke(new Action(() => btn_inicia.Text = "Iniciar"));
            dadotxt.WriteLine(""); // Escrevendo nova linha de dado
            dadotxt.WriteLine("COLETA FINALIZADA " + DateTime.Now.Day + "/" + DateTime.Now.Month + "/" + DateTime.Now.Year + " - " + DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second); //.ToShortDateString() + " - " + DateTime.Now.ToShortTimeString()); // Escrevendo nova linha de dado
            dadotxt.WriteLine("");
            dadotxt.Close();
            MessageBox.Show("Dados Salvos.", "Salvo!", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }
        public void GravaDados()
        {
            while (run)
            {
                while (BFFCGrav.cont > 3)
                {
                    if (flag_buffer)
                    {
                        mut.WaitOne(); // Comando para aguardar conincidencia de Thread
                                       //dadotxt = System.IO.File.AppendText(Nome_arquivo);
                        dadotxt.WriteLine(Convert.ToString(BFFCGrav.read()) + "  "
                            + Convert.ToString(BFFCGrav.read()) + "  "
                            + Convert.ToString(BFFCGrav.read()) + "  "
                            + Convert.ToString(BFFCGrav.read())); // Escrevendo nova linha de dado
                        mut.ReleaseMutex(); // Libera outra Thread
                    }
                }
            }
            //FinalizaColeta();
            //Finish2 = true;
        }
        public void PlotMovimentacao()
        {
            int n = 0; // Contagem do numero de alcances
            int e = 0; // Numero de etapas
            int pistaV = randomSide1[n];
            bool flag_fora = false;
            int cont = 0;
            while (run)
            {
                mut.WaitOne(); // Comando para aguardar conincidencia de Thread
                if (BFFCplot.cont > 3)
                {
                    if (flag_buffer)
                    {
                        Posicao1 = BFFCplot.read();
                        Posicao2 = BFFCplot.read();
                        Corrente1 = BFFCplot.read();
                        Corrente2 = BFFCplot.read();
                        cont++;
                    }
                    //filtro_media_movel(Posicao1, Posicao2);
                    //pbManopla.Invoke(new Action(() => pbManopla.Left = Posicao1 * FatorMov));
                    //pbManopla.Invoke(new Action(() => pbManopla.Top = pbMovArea.Height - pbManopla.Height - (Posicao2 * FatorMov) - 1));
                    if (cont > 5) // garante que sera plotado na velocidade adequada
                    {
                        cont = 0;
                        pbManopla.Invoke(new Action(() => pbManopla.Location = new Point(Posicao1 * FatorMov, pbMovArea.Height - pbManopla.Height - (Posicao2 * FatorMov) - 1)));
                    }
                        //pbManopla.Invoke(new Action(() => pbManopla.Location = new Point(ppx * FatorMov, pbMovArea.Height - pbManopla.Height - (ppy * FatorMov) - 1)));

                    //Console.WriteLine("x: " + (Posicao1 * FatorMov).ToString() + " y: " + (pbMovArea.Height - pbManopla.Height - (Posicao2 * FatorMov) - 1).ToString());
                    //lbl1.Invoke(new Action(() => lbl1.Text = Convert.ToString(Corrente1)));
                    //lbl2.Invoke(new Action(() => lbl2.Text = Convert.ToString(n)));
                    //lbl3.Invoke(new Action(() => lbl_mensagem.Text = Convert.ToString(Corrente1)));
                    //lbl4.Invoke(new Action(() => lbl4.Text = Convert.ToString(Corrente2)));

                    if (!flag_fora) // Só conta um novo alcance quando retorna ao inicio
                    {
                        if (pbManopla.Location.Y > pbMovArea.Height * 0.95)
                        {
                            flag_fora = true;
                            if ((e > 1) && (e < 8))  // Não mosta na baseline e Readapt
                            {
                                var dataByte = new byte[] { 0x7E, 0x73 };
                                serialPort1.Write(dataByte, 0, 2);
                                //g.Invoke(new Action(() =>
                                //g.Clear(Color.LightGray);//));// apaga pista visual
                                //a.Clear(Color.LightGray); // apaga
                                //if ((e > 1) && (e < 5)) // Não mosta na baseline e Readapt
                                //{
                                   // if (flag_pistaVisual == 1)
                                        PistaVisualTracejada(pistaV); // Visualiza pista visual (DEVOLVER)
                                //    else if (flag_pistaVisual == 2)
                                //        AlvoCircular(pistaV);
                                //}
                            }
                            else if(e > 10)
                            {
                                var dataByte = new byte[] { 0x7E, 0x73 };
                                serialPort1.Write(dataByte, 0, 2);
                                //if (flag_pistaVisual == 1)
                                   // PistaVisualTracejada(pistaV); // Visualiza pista visua
                            }
                            else
                            {
                                var dataByte = new byte[] { 0x7E, 0x70 };
                                serialPort1.Write(dataByte, 0, 2);
                                if (e > 7) // DEVOLVER
                                    g.Clear(Color.LightGray);//));// apaga pista visual // DEVOLVER
                            }
                        }
                    }
                    if ((pbManopla.Location.Y < (pbAlvo.Location.Y + (pbAlvo.Height / 2))) && (pbManopla.Location.Y > pbAlvo.Location.Y) &&
                        (pbManopla.Location.X < (pbAlvo.Location.X + (pbAlvo.Width / 2))) && (pbManopla.Location.X >= pbAlvo.Location.X))
                    {
                        if (flag_fora) // garante a contagem do alcance
                        {
                            flag_fora = false;
                            

                            pbAcerto.BackColor = Color.Green;
                            pbAlvo.BackColor = Color.Green;
                            
                            //serialPort1.Write(Convert.ToString(0x7E));
                            //serialPort1.Write(Convert.ToString(0x72));// Envia comando para fim do alcance
                            n++;
                            pistaV = randomSide1[n]; // Gera nova pista visual aleatória
                            MovimentoN(n);
                            
                            if (n > alcances - 1) // 100 tentativas de alcance
                            {
                                e++;
                                FimEtapaN(e);
                                n = 0;
                                if (e > etapas - 1) // 5 Fases (Base line
                                {
                                    FinalizaColeta();
                                }
                            }
                            if ((e > 1) && (e < 8))
                            {
                                var dataByte = new byte[] { 0x7E, 0x71 };
                                serialPort1.Write(dataByte, 0, 2);
                            }
                            else if(e > 10)
                            {
                                var dataByte = new byte[] { 0x7E, 0x71 };
                                serialPort1.Write(dataByte, 0, 2);
                            }
                            else
                            {
                                var dataByte = new byte[] { 0x7E, 0x70 };
                                serialPort1.Write(dataByte, 0, 2);
                            }
                        }
                    }
                    else
                    {
                        if (flag_fora)
                        {
                            pbAcerto.BackColor = Color.Red;
                            pbAlvo.BackColor = Color.DimGray;
                        }
                    }
                }
                mut.ReleaseMutex(); // Libera outra Thread
            }
            //Finish3 = true;
        }
        //Alvo circular posição (x,y)
        private void AlvoCircular(int _sense)
        {
            //a.Clear(Color.LightGray); // apaga
            //alvoPen.Width = 1; //  largura da linha
            a = pbMovArea.CreateGraphics();
            RectangleF rect;
            if (_sense == 1)
                rect = new RectangleF(Convert.ToInt16(pbMovArea.Width * 0.1), Convert.ToInt16(pbMovArea.Height * 0.9), 15.0F, 15.0F); // (posição X, posição Y, diametro horizontal, diametro vertical)
            else
                rect = new RectangleF(Convert.ToInt16(pbMovArea.Width * 0.9), Convert.ToInt16(pbMovArea.Height * 0.9), 15.0F, 15.0F); // (posição X, posição Y, diametro horizontal, diametro vertical)

            a.DrawEllipse(alvoPen, rect);
        }
        // Pistas visuais tracejadas (true - direita / false - esquerda)
        private void PistaVisualTracejada(int _sense)
        {
            //g.Clear(Color.LightGray);// apara
            //Teste posição do alvo
            //AlvoPX = Convert.ToInt16(pbMovArea.Width * 0.75);
            //AlvoPY = Convert.ToInt16(pbMovArea.Height * 0.75);

            float[] dashValues = { 10, 5, 10, 5 };
            myPen.DashPattern = dashValues;

            //myPen.Width = 1; //  largura da linha
            g = pbMovArea.CreateGraphics();
            g.Clear(Color.LightGray);//));// apaga pista visual

            end_y = Convert.ToInt16(pbMovArea.Height * 0.1);
            if (_sense == 1)
                end_x = Convert.ToInt16(pbMovArea.Width / 2) + Convert.ToInt16((pbMovArea.Width * 0.4) * Math.Sin(end_y * Math.PI / (pbMovArea.Width * 0.9)));
            else
                end_x = Convert.ToInt16(pbMovArea.Width / 2) - Convert.ToInt16((pbMovArea.Width * 0.4) * Math.Sin(end_y * Math.PI / (pbMovArea.Width * 0.9)));

            for (int i = Convert.ToInt16(pbMovArea.Height * 0.1); i < Convert.ToInt16(pbMovArea.Height * 0.9); i += 25)
            {
                start_x = end_x;
                start_y = end_y;

                end_y = i;// Convert.ToInt16(start_y + Math.Sin(i) * pbMovArea.Height * 0.9);
                if (_sense == 1)
                    end_x = Convert.ToInt16(pbMovArea.Width / 2) + Convert.ToInt16((pbMovArea.Width * 0.4) * Math.Sin(i * Math.PI / (pbMovArea.Width * 0.9)));
                else
                    end_x = Convert.ToInt16(pbMovArea.Width / 2) - Convert.ToInt16((pbMovArea.Width * 0.4) * Math.Sin(i * Math.PI / (pbMovArea.Width * 0.9)));

                Point[] points =
                {
                new Point (start_x, start_y),
                new Point (end_x, end_y)
                };
                g.DrawLines(myPen, points);
            }
        }
    }

    /*
const int n = 100;
int[] px = new int[n];
int[] py = new int[n];
int ppx, ppy;
private void filtro_media_movel(int _x, int _y) // sinal de entrada 
{
    for (int i = n-1; i > 0; i--)
    {// Desloca elementos do vetor de média móvel
        px[i] = px[i - 1];
        py[i] = px[i - 1];
    }
    px[0] = _x; // Posição inicial do vetor que recebe leitura
    py[0] = _y;
    ppx = 0; // Acumulador para somatória
    ppy = 0;
    for (int i = 0; i < n; i++) // Faz a somatória do vetor
    {
        ppx += px[i];
        ppy += px[i];
    }
    ppx = ppx / n;
    ppy = ppy / n;
}
*/
}
