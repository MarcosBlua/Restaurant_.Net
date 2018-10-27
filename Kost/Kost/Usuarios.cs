﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using CapaNegocio;


namespace Kost
{
    public partial class Usuarios : UserControl
    {
        Boolean banderaGuardar = true;
        public Usuarios()
        {
            InitializeComponent();
            dgvUsuarios.DataSource = Usuario.ListarTodos();

            DataTable dt = new DataTable();
            dt.Clear();
            dt.Columns.Add("id");
            dt.Columns.Add("Nivel");
            DataRow _ravi = dt.NewRow();
            _ravi["id"] = "1";
            _ravi["Nivel"] = "Admin";
            dt.Rows.Add(_ravi);

            _ravi = dt.NewRow();
            _ravi["id"] = "2";
            _ravi["Nivel"] = "User";
            dt.Rows.Add(_ravi);

            cbxNivel.DataSource = dt.DefaultView;
            cbxNivel.ValueMember = "id";
            cbxNivel.DisplayMember = "Nivel";
            cbxNivel.BindingContext = this.BindingContext;

            pnlUsuario.Enabled = false;
            dgvUsuarios.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }


        //Botones
        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (banderaGuardar)
            {
                CapaNegocio.Usuario usuario = new CapaNegocio.Usuario(txtUsuario.Text, txtContraseña.Text, Convert.ToInt16(cbxNivel.SelectedValue), txtNombre.Text, txtApellido.Text, txtDireccion.Text, txtMail.Text, Convert.ToInt64(txtCuil.Text.Replace("-", "")), dtpNacimiento.Value);
                if (usuario.Error)
                {
                    if(usuario.Mensaje == "Persona no activa")
                    {
                        if (CapaNegocio.Funciones.mConsulta(this, "Existe un usuario no activo con este cuil, ¿Desea ver esos datos para soobreescribirlos?, de ser la respuesta no se creara un nuevo usuario con los datos que ingreso"))
                        {
                            banderaGuardar = false;
                            MostrarDatosDeUsuario(usuario.Cuil);
                        }
                        else
                        {
                            GuardarModificacion();
                        }
                    }
                    else
                    {
                        CapaNegocio.Funciones.mError(this, usuario.Mensaje);
                    }
                }
                else
                {
                    CapaNegocio.Funciones.mOk(this, "Se guardo el usuario exitosamente");
                    dgvUsuarios.DataSource = Usuario.ListarTodos();
                    Clear();
                    pnlUsuario.Enabled = false;
                }   
            }
            else
            {
                GuardarModificacion();
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            Clear();
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            Clear();
            if (RowSeleccionado(dgvUsuarios.SelectedRows.Count, "eliminarlo."))
            {
                if (CapaNegocio.Funciones.mConsulta(this, "¿Esta seguro de que desea eliminar el usuario?"))
                {
                    long cuil = Convert.ToInt64(dgvUsuarios.CurrentRow.Cells["cuil"].Value);
                    if (Usuario.Eliminar(cuil))
                    {
                        Funciones.mOk(this, "Se eliminó correctamente al usuario.");
                        dgvUsuarios.DataSource = Usuario.ListarTodos();
                    }
                    else
                    {
                        Funciones.mError(this, "Error al eliminar al usuario.");
                    }
                    Clear();
                    pnlUsuario.Enabled = false;
                }
            }
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            Clear();
            if (RowSeleccionado(dgvUsuarios.SelectedRows.Count, "modificarlo."))
            {
                pnlUsuario.Enabled = true;
                btnCancelar.Enabled = true;
                btnGuardar.Enabled = true;
                banderaGuardar = false;

                long cuil = Convert.ToInt64(dgvUsuarios.CurrentRow.Cells["cuil"].Value);
                Usuario user = CapaNegocio.Usuario.TraerUnUsuario(cuil);

                txtApellido.Text = user.Apellido;
                txtContraseña.Text = user.Password;
                txtCuil.Text = user.Cuil.ToString();
                txtDireccion.Text = user.Direccion;
                txtMail.Text = user.Mail;
                txtNombre.Text = user.Nombre;
                txtUsuario.Text = user.User;
                cbxNivel.SelectedValue = user.Nivel;
                dtpNacimiento.Value = user.Nacimiento;
            }
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            Clear();
            pnlUsuario.Enabled = true;
            btnCancelar.Enabled = true;
            btnGuardar.Enabled = true;
        }

        private void btnVerUsuario_Click(object sender, EventArgs e)
        {
            Clear();
            if (RowSeleccionado(dgvUsuarios.SelectedRows.Count, "ver los detalles del mismo."))
            {
                pnlUsuario.Enabled = true;

                btnCancelar.Enabled = false;

                btnGuardar.Enabled = false;

                long cuil = Convert.ToInt64(dgvUsuarios.CurrentRow.Cells["cuil"].Value);

                MostrarDatosDeUsuario(cuil);
            }
        }


        //TextBoxs
        private void txtNombre_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = soloLetras(e.KeyChar);
        }
                
        private void txtApellido_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = soloLetras(e.KeyChar);
        }

        private void txtMail_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = sinEspacios(e.KeyChar);
        }

        private void txtUsuario_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = sinEspacios(e.KeyChar);
        }

        private void txtContraseña_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = sinEspacios(e.KeyChar);
        }


        //Métodos      
        public void Clear()
        {
            txtApellido.Text = "";
            txtContraseña.Text = "";
            txtCuil.Text = "";
            txtDireccion.Text = "";
            txtMail.Text = "";
            txtNombre.Text = "";
            txtUsuario.Text = "";
            dtpNacimiento.Value = DateTime.Today;
            cbxNivel.SelectedValue = 1;
            pnlUsuario.Enabled = false;
        }

        private Boolean soloLetras(char e)
        {
            Boolean result = false;
            if (Char.IsLetter(e))
            {
                result = false;
            }
            else if (Char.IsControl(e))
            {
                result = false;
            }
            else if (Char.IsSeparator(e))
            {
                result = false;
            }
            else
            {
                result = true;
            }

            return result;
        }        

        private Boolean sinEspacios(char e)
        {
            

            Boolean result = false;
            if (Char.IsSeparator(e))
            {
                result = true;
            }
            else
            {
                result = false;
            }

            return result;
        }        

        private void MostrarDatosDeUsuario(long cuil)
        {
            Usuario user = CapaNegocio.Usuario.TraerUnUsuario(cuil);

            txtApellido.Text = user.Apellido;
            txtContraseña.Text = user.Password;
            txtCuil.Text = user.Cuil.ToString();
            txtDireccion.Text = user.Direccion;
            txtMail.Text = user.Mail;
            txtNombre.Text = user.Nombre;
            txtUsuario.Text = user.User;
            cbxNivel.SelectedValue = user.Nivel;
            dtpNacimiento.Value = user.Nacimiento;
        }

        private void GuardarModificacion()
        {
            if (CapaNegocio.Usuario.ModificarUsuario(txtUsuario.Text, txtContraseña.Text, Convert.ToInt32(cbxNivel.SelectedValue), Convert.ToInt64(txtCuil.Text.Replace("-", "")), txtNombre.Text, txtApellido.Text, txtDireccion.Text, txtMail.Text, dtpNacimiento.Value))
            {
                CapaNegocio.Funciones.mOk(this, "Los cambios al usuario se guardaron correctamente");
                dgvUsuarios.DataSource = Usuario.ListarTodos();
                banderaGuardar = true;
                Clear();
                pnlUsuario.Enabled = false;
            }
            dgvUsuarios.DataSource = Usuario.ListarTodos();
        }

        private Boolean RowSeleccionado(int c, string msj)
        {
            if (c == 1)
            {
                return true;
            }
            else if (c == 0)
            {
                Funciones.mAdvertencia(this, "Debe seleccionar un usuario para poder " + msj);
                return false;
            }
            else
            {
                Funciones.mAdvertencia(this, "Debe seleccionar sólo un usuario.");
                return false;
            }
        }
    }
}