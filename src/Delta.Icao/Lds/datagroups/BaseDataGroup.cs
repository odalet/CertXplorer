////using System;
////using System.IO;
////using System.Linq;
////using System.Text;
////using System.Collections.Generic;
////using Delta.Icao.Logging;

////////using Siti.Icao.Asn1;
////////using Siti.Logging;

////namespace Delta.Icao.Lds
////{
////    public abstract class BaseDataGroup
////    {
////        private List<DataGroupVerificationResult> verificationResults = new List<DataGroupVerificationResult>();
////        private static readonly ILogService log = LogManager.GetLogger<BaseDataGroup>();

////        public BaseDataGroup(DataGroupFileIdentifier fileIdentifier, byte[] data)
////        {
////            FileIdentifier = fileIdentifier;
////            Data = data ?? new byte[0];
////        }

////        public DataGroupFileIdentifier FileIdentifier { get; private set; }

////        public byte[] Data { get; private set; }

////        public bool Parsed { get; private set; }

////        public bool Verified { get; private set; }

////        public IReadOnlyCollection<DataGroupVerificationResult> VerificationResults
////        {
////            get
////            {
////                if (!Verified) Verify();
////                return verificationResults.MakeReadOnly();
////            }
////        }

////        #region Diagnostics

////        public string Log(string tab = "")
////        {
////            var builder = new StringBuilder();
////            LogToBuilder(builder, tab);
////            LogVerificationResults(builder, tab);
////            return builder.ToString();
////        }

////        protected virtual void LogToBuilder(StringBuilder builder, string tab) { }
        
////        public virtual bool SaveToDirectory(string directory, string suffix = "")
////        {
////            if (string.IsNullOrEmpty(directory)) throw new ArgumentNullException("directory");
////            if (!Directory.Exists(directory)) throw new ArgumentException(string.Format(
////                "Directory {0} does not exist", directory), "directory");

////            // Save Binary data
////            var filename = GetFileNameToSave(directory, suffix);            
////            try
////            {
////                var bytes = Data ?? new byte[0];
////                File.WriteAllBytes(filename, bytes);
////            }
////            catch (Exception ex)
////            {
////                log.Warning(string.Format("Could not write Datagroup ({0}) data file {1}: {2}", 
////                    FileIdentifier, filename, ex.Message), ex);
////                return false;
////            }

////            // Save Log (if not empty)
////            var logData = Log();
////            if (string.IsNullOrEmpty(logData)) return true;

////            filename = GetFileNameToSave(directory, suffix, ".txt");
////            try
////            {
////                File.WriteAllText(filename, logData);
////            }
////            catch (Exception ex)
////            {
////                log.Warning(string.Format("Could not write MRZ to file {0}: {1}",
////                    filename, ex.Message), ex);
////                return false;
////            }

////            return true;
////        }

////        protected string GetFileNameToSave(string directory, string suffix, string extension = ".bin")
////        {
////            return Path.Combine(directory, string.IsNullOrEmpty(suffix) ?
////                FileIdentifier.ToString() + extension :
////                FileIdentifier.ToString() + suffix + extension);
////        }

////        #endregion

////        #region DG Parsing

////        // Parses the DG raw data.
////        public void Parse(bool checkParsedData = true)
////        {
////            Parsed = false;
////            try
////            {
////                ParseData();
////                Parsed = true;
////            }
////            catch (Exception ex)
////            {
////                AddError("DataGroup Parsing Error", string.Format("Could not parse DataGroup {0}.", FileIdentifier), ex);
////                log.Error(string.Format("Could not parse this Data Group {0} content: {1}", FileIdentifier, ex.Message), ex);
////            }

////            if (checkParsedData) Verify();
////        }

////        protected virtual void ParseData() { }

////        #endregion

////        #region DG Verification

////        // Validates the DG is correct
////        protected virtual void Verify()
////        {
////            Verified = false;
////            try
////            {
////                VerifyParsedData();
////                Verified = true;
////            }
////            catch (Exception ex)
////            {
////                AddError("DataGroup Verification Error", string.Format("Could not complete DataGroup {0} Verifications.", FileIdentifier), ex);
////                log.Error(string.Format("Could not complete this Data Group {0} verification: {1}", FileIdentifier, ex.Message), ex);
////            }
////        }

////        protected virtual void VerifyParsedData() { }

////        private void LogVerificationResults(StringBuilder builder, string tab = "")
////        {
////            if (VerificationResults == null || VerificationResults.Count == 0)
////                return;

////            builder.AppendLine(tab + "DG Verification Results:");
////            var t = tab + "* ";
////            const string twelveWhites = "            ";
////            foreach (var result in VerificationResults.OrderBy(r => r.Level))
////            {
////                if (string.IsNullOrEmpty(result.DisplayName)) builder.AppendLine(
////                    t + result.Level.ToString());
////                else builder.AppendLine(
////                    t + string.Format("{0} - {1}", result.Level.ToString().PadRight(7), result.DisplayName));

////                if (!string.IsNullOrEmpty(result.Description) && result.Description != result.DisplayName)
////                    builder.AppendLine(tab + twelveWhites + result.Description);

////                if (result.Exception != null)
////                {
////                    var exString = ("Exception Details: " + result.Exception.ToFormattedString())
////                        .Replace('\r', '\n')
////                        .Replace("\n\n", "\n")
////                        .Replace("\n", "\r\n" + twelveWhites + "  ");
////                    builder.AppendLine(exString);
////                }
////            }
////        }

////        #endregion

////        #region Messages Management

////        protected void AddWarning(string nameAndDescription, Exception exception = null)
////        {
////            AddCheckResult(DataGroupVerificationResultLevel.Warning, nameAndDescription, nameAndDescription, exception);
////        }

////        protected void AddWarning(string name, string description, Exception exception = null)
////        {
////            AddCheckResult(DataGroupVerificationResultLevel.Warning, name, description, exception);
////        }

////        protected void AddError(string nameAndDescription, Exception exception = null)
////        {
////            AddCheckResult(DataGroupVerificationResultLevel.Error, nameAndDescription, nameAndDescription, exception);
////        }

////        protected void AddError(string name, string description, Exception exception = null)
////        {
////            AddCheckResult(DataGroupVerificationResultLevel.Error, name, description, exception);
////        }

////        private void AddCheckResult(DataGroupVerificationResultLevel level, string name, string description, Exception exception)
////        {
////            var logLevel = level.ToLogLevel();
////            log.Log(logLevel, string.Format("{0}; {1}", name, description), exception);

////            // Create and add the result object
////            verificationResults.Add(new DataGroupVerificationResult()
////            {
////                Level = level,
////                DisplayName = name,
////                Description = description,
////                Exception = exception
////            });
////        }

////        #endregion

////        #region ASN.1 Helpers

////        protected byte[] RemoveFileDelimiter(byte[] data)
////        {
////            // Determine data length            
////            var lengthLength = 0;
////            var length = data.LeftTruncate(1).LengthDecode(out lengthLength);
////            return data.LeftTruncate(lengthLength + 1).RightTruncate(length);
////        }

////        #endregion
////    }
////}
