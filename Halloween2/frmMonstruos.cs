using System.Data;
using System.Diagnostics.Contracts;

namespace Halloween2
{
    public partial class frmMonstruos : Form
    {
        public frmMonstruos()
        {
            InitializeComponent();
        }

        #region Enumerado
        public enum ModoEdicion
        {
            lectura,
            crear,
            modificar
        }

        public ModoEdicion modoEdicion;
        #endregion

        #region Eventos
        private void btnAñadir_Click(object sender, EventArgs e)
        {
            modoEdicion = ModoEdicion.crear;
            ModoPantallaEdicion();
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            bool correcto = false;
            DialogResult respuesta = MessageBox.Show("¿Está seguro de que desea eliminar el registro?", "Confirmación", MessageBoxButtons.YesNo);

            if (respuesta == DialogResult.Yes)
            {
                Monstruos c = ObtenerInformacion();
                correcto = Repositorio.EliminarMonstruo(c);

                if (correcto)
                {
                    MessageBox.Show("la acción se ha realizado correctamente.");

                    ModoPantallaLectura();

                    CargarYConfigurarGrid();
                }
            }
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            modoEdicion = ModoEdicion.modificar;
            ModoPantallaEdicion();


        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            bool correcto = false;

            if (InformacionObligatoriaCumplimentada())
            {

                Monstruos c = ObtenerInformacion();

                switch (modoEdicion)
                {
                    case ModoEdicion.crear:
                        correcto = Repositorio.CrearMonstruo(c);
                        break;
                    case ModoEdicion.modificar:
                        correcto = Repositorio.ModificarMonstruo(c);
                        break;
                }


                if (correcto)
                {
                    MessageBox.Show("la acción se ha realizado correctamente.");
                    modoEdicion = ModoEdicion.lectura;

                    ModoPantallaLectura();

                    CargarYConfigurarGrid();
                }

            }
            else
            {
                MessageBox.Show("Los campos Nombre y Fecha de Inscripción son obligatorios.");
            }


        }



        private void btnCancelar_Click(object sender, EventArgs e)
        {
            DialogResult respuesta = MessageBox.Show("¿Está seguro de que desea salir de la edición?", "Confirmación", MessageBoxButtons.YesNo);

            if (respuesta == DialogResult.Yes)
            {
                modoEdicion = ModoEdicion.lectura;
                ModoPantallaLectura();


                if (dgvListado.SelectedRows.Count == 1)
                {

                    CargarInfoFilaSeleccionadaFormulario(dgvListado.SelectedRows[0]);
                }
            }
        }


        private void dgvListado_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            dgvListado.Rows[e.RowIndex].Selected = true;
        }

 
        private void dgvListado_RowStateChanged(object sender, DataGridViewRowStateChangedEventArgs e)
        {
           
            if (e.StateChanged != DataGridViewElementStates.Selected) return;

            
            DataGridViewRow filaSeleccionada = e.Row;
            CargarInfoFilaSeleccionadaFormulario(filaSeleccionada);

        }

        #endregion

        #region Métodos

     
        public void ModoPantallaEdicion()
        {
            
            if (modoEdicion == ModoEdicion.crear)
            {
                txtNombre.Text = "";
                dtpFechaInscripcion.Value = DateTime.Now;
                txtsustos.Text = "";
                txtId.Text = "";

            }

            btnGuardar.Enabled = true;
            btnCancelar.Enabled = true;
            txtNombre.Enabled = true;
            dtpFechaInscripcion.Enabled = true;
            txtsustos.Enabled = true;
            txtId.Enabled = false;

            btnAñadir.Enabled = false;
            btnEliminar.Enabled = false;
            btnModificar.Enabled = false;

            dgvListado.Enabled = false;
        }

       
        public void ModoPantallaLectura()
        {
            txtNombre.Text = "";
            dtpFechaInscripcion.Value = DateTime.Now;
            txtsustos.Text = "";
            txtId.Text = "";

            btnGuardar.Enabled = false;
            btnCancelar.Enabled = false;
            btnAñadir.Enabled = true;
            btnEliminar.Enabled = true;
            btnModificar.Enabled = true;

            txtNombre.Enabled = false;
            dtpFechaInscripcion.Enabled = false;
            txtsustos.Enabled = false;
            txtId.Enabled = false;

            dgvListado.Enabled = true;
        }

       
        public Monstruos ObtenerInformacion()
        {
            Monstruos monstruo = new Monstruos();

            monstruo.nombre = txtNombre.Text;
            monstruo.sustos = (int) Convert.ChangeType(txtsustos.Text, typeof(int));
            monstruo.fechaDeInscripcion = dtpFechaInscripcion.Value;

            if (!String.IsNullOrEmpty(txtId.Text))
            {
                monstruo.ID = Convert.ToInt32(txtId.Text);
            }

            return monstruo;
        }

       
        public void CargarYConfigurarGrid()
        {
            DataSet ds = Repositorio.ObtenerMonstruos();
            dgvListado.DataSource = ds.Tables[0];

            // Tamaños columnas
            dgvListado.Columns["Id"].Width = 40;
            dgvListado.Columns["Nombre"].Width = 150;
            dgvListado.Columns["FechaDeInscripcion"].Width = 150;
            dgvListado.Columns["Sustos"].Width = 120;

            // Renombrado columnas
            dgvListado.Columns["FechaDeInscripcion"].HeaderText = "Fecha Inscripcion";

            // Formato fecha en español
            dgvListado.Columns["FechaDeInscripcion"].DefaultCellStyle.Format = "dd/MM/yyyy";

            // Seleccionamos la primera fila del grid si existe
            SeleccionarPrimeraFilaGrid();

        }

        public void SeleccionarPrimeraFilaGrid()
        {
            // Si hay alguna fila, seleccionamos la primera
            if (dgvListado.Rows.Count > 0)
            {
                dgvListado.Rows[0].Selected = true;
            }
        }

        
        public void CargarInfoFilaSeleccionadaFormulario(DataGridViewRow filaSeleccionada)
        {
            txtId.Text = filaSeleccionada.Cells["Id"].Value.ToString();
            txtNombre.Text = filaSeleccionada.Cells["Nombre"].Value.ToString();
            dtpFechaInscripcion.Value = (DateTime)filaSeleccionada.Cells["FechaDeInscripcion"].Value;
            txtsustos.Text = filaSeleccionada.Cells["Sustos"].Value.ToString();
       

        }

     
        public bool InformacionObligatoriaCumplimentada()
        {
            if (String.IsNullOrEmpty(txtNombre.Text) || String.IsNullOrEmpty(txtsustos.Text))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        #endregion

        

    }

}

        
