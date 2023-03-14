using BibliotecaBanco;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ConsultaCredito
{
    public partial class frmconsultaCredito : Form
    {
        private FileStream entrada; //MAntiene la conexion con el archivo
        private StreamReader archivoReader; //Leer los datos de texto

        private string nombreArchivo;
        public frmconsultaCredito()
        {
            InitializeComponent();
        }

        private void btnAbrir_Click(object sender, EventArgs e)
        {
            OpenFileDialog selectorArchivo = new OpenFileDialog();
            DialogResult resultado = selectorArchivo.ShowDialog();

            if (resultado == DialogResult.Cancel)
                return;

            nombreArchivo = selectorArchivo.FileName;

            if (nombreArchivo == "" || nombreArchivo == null)
            {
                MessageBox.Show("Nombre del archivo invalido", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                entrada = new FileStream(nombreArchivo, FileMode.Open, FileAccess.Read);
                archivoReader = new StreamReader(entrada);

                btnAbrir.Enabled = false;
                btnCredito.Enabled = true;
                btnDebito.Enabled = true;
                btnCero.Enabled = true;
            }
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            if (entrada != null)
            {
                try
                {
                    entrada.Close();
                    archivoReader.Close();
                }
                catch (IOException)
                {
                    MessageBox.Show("No se pudo crear el archivo", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }Application.Exit();
        }

        //Se invoca cuando el usuario hace clic en el boton de saldo con credito,
        //Saldo con debito o saldos en cero
        private void ObtenerSaldo_Click(object sender, EventArgs e) 
        {
            //conviertee el emisor (sender) explicitamente a un objeto de tipo Button
            Button emisorButton = (Button)sender;

            //obtiene el texto del boton en el que se hizo clic, y que almacena el tipo 
            //de la cuenta
            string tipoCuenta = emisorButton.Text;

            //leer y muestra la informacion del archivo


            try
            {
                //regresa al principio
                entrada.Seek(0, SeekOrigin.Begin);

                txtMostrar.Text = "Las cuentasson:\r\n";

                //Recorre el archivo hasta llegassr a su fin
                while (true)
                {
                    string[] camposEntrada; //almacena piezas de datos indicviduales
                    Registro registro;  //Almacena cada regiastro a medida que se lee el archivo
                    decimal saldo;  //Almacena el saldo a cada Registro

                    //obtiene el siguiente Registro disponible en el archivo
                    string registroEntrada = archivoReader.ReadLine();

                    //cuando esta al final del archivo, sale del metodo

                    if (registroEntrada == null)
                        return;

                    camposEntrada = registroEntrada.Split(','); //Analiza la entrada

                    //Crea el registro a partir de entrada
                    registro = new Registro(Convert.ToInt16(camposEntrada[0]), camposEntrada[1], camposEntrada[2], Convert.ToDecimal(camposEntrada[3]));

                    //Almacenar el ultimo campo del regisdtro en saldo
                    saldo = registro.Saldo;

                    //Metodo parta determinar si mueesra saldo o no
                    if (DebeMostrar(saldo, tipoCuenta))
                    {

                        //muestra el refistro
                        string salida = registro.Cuenta + "\t" + registro.PrimerNombre + "\t" +
                            registro.ApellidoPaterno + "\t";

                        //muiestra el saldo con el formato monetario correccto
                        salida += String.Format("{0:F}", saldo) + "\r\n";

                        txtMostrar.Text += salida;
                    }
                }
            }
            //maneja la excepcion cuando no puede leeerse el archivo
            catch (IOException)
            {
                MessageBox.Show("No se pude leer el archivo", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool DebeMostrar(decimal saldo, string tipoCuenta)
        {
            if (saldo > 0)
            {
                //Muestra los saldos con credito
                if (tipoCuenta == "Saldos con creditos")
                    return true;

            }else if (saldo < 0)
            {
                //muestra los valores con debitos
                if (tipoCuenta == "Saldos con debito")
                    return true;
            }
            else //saldo == 0
            {
                if (tipoCuenta == "Saldos en cero")
                    return true;
            }
            return false;
        }
    }
}
