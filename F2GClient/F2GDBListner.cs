﻿using System;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using F2G.Models;
using System.ComponentModel;
using System.Threading;

namespace F2GClient {
    class F2GDBListner {
        private string ipAddr;
        private BackgroundWorker bw;
        private string fileToBeFound;
        public event EventHandler FileFound;

        F2GDBListner (string ip) {
            fileToBeFound = "";
            ipAddr = ip;
            bw = new BackgroundWorker();
            bw.WorkerSupportsCancellation = true;
        }

        public void CheckQueue() {
            while (fileToBeFound == "") {
                if (!bw.IsBusy) {
                    bw.DoWork += bw_DoWork;
                    bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);
                    bw.RunWorkerAsync();
                }
            }
        }

        private void bw_DoWork(object sender, DoWorkEventArgs e) {
            fileToBeFound = "";
            using (F2GContext db = new F2GContext()) {
                Request req = db.Requests.FirstOrDefault(r => r.client.ip == ipAddr);
                if (req != null) {
                    fileToBeFound = req.fileName;
                    OnFileFound(new FileFoundEventArgs { FileName = fileToBeFound });
                }
            }
        }
        public class FileFoundEventArgs : EventArgs {
            public string FileName { get; set; }
        }

        public delegate void FileFoundEventHandler(FileFoundEventArgs e);

        protected virtual void OnFileFound(EventArgs e) {
            EventHandler handler = FileFound;
            if (handler != null) {
                handler(this, e);
            }
        }

        private void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            if (fileToBeFound != "") {

            }
        }
    }
}
