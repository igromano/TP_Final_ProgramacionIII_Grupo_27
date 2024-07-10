﻿using ManoExperta.helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using dominio;
using negocio;
using System.Web.Services.Description;

namespace ManoExperta
{
    public partial class ListadoTickets : System.Web.UI.Page
    {
        public Usuario usuariotemp = new Usuario();
        public TrabajoNegocio trabajoTemp = new TrabajoNegocio();
        public ServicioNegocio servicioNegocioTemp = new ServicioNegocio();
        public List<Estado> estadosTemp = new List<Estado>();
        public List<Ticket> ticketsTemp = new List<Ticket>();
        public List<Locacion> locacionTemp = new List<Locacion>();
        public List<Locacion> provinciasUnicas = new List<Locacion>();
        public List<Especialidad> especialidadTemp = new List<Especialidad>();
        public List<Ticket> ticketsFiltro = new List<Ticket>();
        public int trabajosActivos = 0;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (AuthServices.estaLogueado((Usuario)Session["usuario"]) == false)
            {
                Response.Redirect("Login.aspx", false);
            }
            else
            {
                if (!IsPostBack)
                {
                    usuariotemp = (Usuario)Session["usuario"];
                    string nombre = usuariotemp.Nombre;
                    if (usuariotemp.RolUsuario != RolUsuario.PRESTADOR)
                    {
                        Response.Redirect("Home.aspx", false);
                    }
                    estadosTemp = servicioNegocioTemp.getEstados();
                    ticketsTemp = trabajoTemp.getTicketsPorEstado(estadosTemp.Find(es => es.Nombre.Equals("A ASIGNAR")));
                    Session["tickets"] = ticketsTemp;

                    locacionTemp = servicioNegocioTemp.getLocaciones();
                    DropDownListLocalidadFiltro.DataSource = locacionTemp;
                    DropDownListLocalidadFiltro.DataTextField = "Nombre";
                    DropDownListLocalidadFiltro.DataValueField = "Id";
                    DropDownListLocalidadFiltro.DataBind();
                    DropDownListLocalidadFiltro.SelectedValue = usuariotemp.IdLocalidad.ToString();

                    ticketsFiltro = new List<Ticket>(ticketsTemp);
                    repTrabajosActivos.DataSource = ticketsFiltro.FindAll(tck => tck.IdLocalidad == usuariotemp.IdLocalidad);
                    repTrabajosActivos.DataBind();

                    provinciasUnicas = locacionTemp.GroupBy(loc => new { loc.IdProvincia, loc.NombreProvincia }).Select(loc => loc.First()).ToList();

                    DropDownListProvinciaFiltro.DataSource = provinciasUnicas;
                    DropDownListProvinciaFiltro.DataTextField = "NombreProvincia";
                    DropDownListProvinciaFiltro.DataValueField = "IdProvincia";
                    DropDownListProvinciaFiltro.DataBind();
                }
                else
                {
                    ticketsTemp = (List<Ticket>)Session["tickets"];
                }
            }


        }

        protected void buttonMasInformacion_Click(object sender, EventArgs e)
        {
            string id = ((Button)sender).CommandArgument;
            Response.Redirect("Detalle.aspx?idTicket=" + id, false);
        }


        protected void DropDownListProvinciaFiltro_SelectedIndexChanged(object sender, EventArgs e)
        {

            ticketsFiltro = new List<Ticket>(ticketsTemp);

            string idSeleccionado = DropDownListProvinciaFiltro.SelectedValue;
            //ticketsFiltro.RemoveAll(tck => !tck..Equals(idSeleccionado));
            repTrabajosActivos.DataSource = ticketsFiltro;
            repTrabajosActivos.DataBind();
        }

        protected void DropDownListLocalidadFiltro_SelectedIndexChanged(object sender, EventArgs e)
        {
            ticketsFiltro = new List<Ticket>(ticketsTemp);
            repTrabajosActivos.DataSource = ticketsFiltro.FindAll(tck => tck.IdLocalidad == int.Parse(DropDownListLocalidadFiltro.SelectedValue));
            repTrabajosActivos.DataBind();
            trabajosActivos = repTrabajosActivos.Items.Count;
        }
    }
}