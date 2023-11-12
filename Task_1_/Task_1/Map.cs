using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Task_1
{
    public partial class Map : Form
    {
        private GMapControl _map;
        private IContainer components = null;
        private readonly DataBase _db;

        private List<DateByMachine> _dateByMachine = new List<DateByMachine>();
        private List<GMarkerGoogle> _gmarkers = new List<GMarkerGoogle>();

        private bool _isDragging;
        private GMarkerGoogle _draggedMarker;

        public Map(DataBase dataBase)
        {
            _db = dataBase;
            InitializeComponent();
            InitializeMap();
            InitializeMarkers();
        }
        private void InitializeComponent()
        {
            components = new Container();
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Text = "Map";
        }
        private void InitializeMap()
        {
            _map = new GMapControl();
            _map.Dock = DockStyle.Fill;
            _map.MapProvider = GMapProviders.GoogleMap;

            _map.Position = new PointLatLng(54.6029846573357, 83.34408051288354);
            _map.MinZoom = 1;
            _map.MaxZoom = 18;
            _map.Zoom = 8;

            _map.MouseUp += Marker_MouseUp;
            _map.MouseDown += Marker_MouseDown;
            _map.MouseMove += Marker_MouseMove;

            Controls.Add(_map);
        }
        private void InitializeMarkers()
        {
            string sqlQuery = "SELECT * FROM MachinePosition";

            _db.OpenConnection();
            using (SqlCommand command = new SqlCommand(sqlQuery, _db.GetConnection()))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        DateByMachine modelMP = new DateByMachine();
                        modelMP.Id = reader.GetInt32(reader.GetOrdinal("Id"));
                        modelMP.PositionX = reader.GetDouble(reader.GetOrdinal("positionX"));
                        modelMP.PositionY = reader.GetDouble(reader.GetOrdinal("positionY"));
                        _dateByMachine.Add(modelMP);
                    }
                }
            }
            _db.CloseConnection();


            foreach (var modelMP in _dateByMachine)
            {
                GMarkerGoogle marker = new GMarkerGoogle(new PointLatLng(modelMP.PositionX, modelMP.PositionY), GMarkerGoogleType.blue);
                GMapOverlay markersOverlay = new GMapOverlay("markers");
                markersOverlay.Markers.Add(marker);

                _gmarkers.Add(marker);

                _map.Overlays.Add(markersOverlay);
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
        }

        private void Marker_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _draggedMarker = GetMarkerAtPosition(e.Location);
                if (_draggedMarker != null)
                {
                    _isDragging = true;
                }
            }
        }
        private void Marker_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isDragging && _draggedMarker != null)
            {
                foreach (var marker in _dateByMachine)
                {
                    if (marker.PositionX == _draggedMarker.Position.Lat && marker.PositionY == _draggedMarker.Position.Lng)
                    {
                        _draggedMarker.Position = _map.FromLocalToLatLng(e.X, e.Y);
                        marker.PositionX = _draggedMarker.Position.Lat;
                        marker.PositionY = _draggedMarker.Position.Lng;
                    }
                }
            }
        }
        private void Marker_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && _isDragging && _draggedMarker != null)
            {

                DateByMachine dateByMachine = _dateByMachine.Where(m => m.PositionX == _draggedMarker.Position.Lat && m.PositionY == _draggedMarker.Position.Lng).FirstOrDefault();

                if (dateByMachine != null)
                {
                    SaveMarkerPosition(_draggedMarker, dateByMachine.Id);

                    _isDragging = false;
                    _draggedMarker = null;
                }
            }
        }

        private GMarkerGoogle GetMarkerAtPosition(Point point)
        {
            foreach (GMarkerGoogle marker in _gmarkers)
            {
                if (marker.IsMouseOver)
                {
                    return marker;
                }
            }

            return null;
        }

        private void SaveMarkerPosition(GMarkerGoogle marker, int markerId)
        {
            double latitude = marker.Position.Lat;
            double longitude = marker.Position.Lng;
            int id = markerId;

            _db.OpenConnection();
            string sqlQuery = "UPDATE MachinePosition SET positionX = @Latitude, positionY = @Longitude WHERE Id = @Id";
            using (SqlCommand command = new SqlCommand(sqlQuery, _db.GetConnection()))
            {
                command.Parameters.AddWithValue("@Latitude", latitude);
                command.Parameters.AddWithValue("@Longitude", longitude);
                command.Parameters.AddWithValue("@Id", id); // Предполагается, что у маркера есть свойство Tag, содержащее идентификатор записи в базе данных

                command.ExecuteNonQuery();
            }
            _db.CloseConnection();
        }



        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}












