using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SteampunkLauncher
{
    public class CrashAnalysis
    {
        public bool HasCrash { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Recommendation { get; set; }
        public string Details { get; set; }
        public string FilePath { get; set; }
    }

    public static class CrashDoctor
    {
        public static void CheckAndShowIfCrashed(IWin32Window owner, string gameDir, int exitCode)
        {
            if (exitCode == 0) return; // Нормальное завершение

            CrashAnalysis analysis = Analyze(gameDir, exitCode);
            if (analysis != null && analysis.HasCrash)
            {
                using (var dlg = new CrashDoctorDialog(analysis))
                {
                    dlg.ShowDialog(owner);
                }
            }
        }

        public static CrashAnalysis Analyze(string gameDir, int exitCode)
        {
            var result = new CrashAnalysis
            {
                HasCrash = true,
                Title = "Игра аварийно завершилась (Код " + exitCode + ")",
                Summary = "Minecraft неожиданно закрылся во время работы.",
                Recommendation = "Нажмите «Скопировать отчет для админа» и отправьте его создателю сервера.",
                Details = ""
            };

            if (string.IsNullOrEmpty(gameDir) || !Directory.Exists(gameDir)) return result;

            string crashReportContent = null;
            string latestLogContent = null;

            // 1. Поиск свежего crash-report (за последние 10 минут)
            string crashDir = Path.Combine(gameDir, "crash-reports");
            if (Directory.Exists(crashDir))
            {
                var latestReport = new DirectoryInfo(crashDir)
                    .GetFiles("crash-*.txt")
                    .OrderByDescending(f => f.LastWriteTime)
                    .FirstOrDefault();

                if (latestReport != null && (DateTime.Now - latestReport.LastWriteTime).TotalMinutes < 10)
                {
                    try
                    {
                        crashReportContent = File.ReadAllText(latestReport.FullName);
                        result.FilePath = latestReport.FullName;
                    }
                    catch { }
                }
            }

            // 2. Чтение хвоста latest.log
            string logPath = Path.Combine(gameDir, "logs", "latest.log");
            if (File.Exists(logPath))
            {
                try
                {
                    var lines = File.ReadAllLines(logPath);
                    int start = Math.Max(0, lines.Length - 100);
                    latestLogContent = string.Join(Environment.NewLine, lines.Skip(start));
                }
                catch { }
            }

            string combined = (crashReportContent ?? "") + "\n" + (latestLogContent ?? "");
            result.Details = !string.IsNullOrEmpty(crashReportContent) ? crashReportContent : latestLogContent;

            // Анализ паттернов
            if (combined.IndexOf("OutOfMemoryError", StringComparison.OrdinalIgnoreCase) >= 0 ||
                combined.IndexOf("insufficient memory", StringComparison.OrdinalIgnoreCase) >= 0 ||
                combined.IndexOf("GC overhead limit", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                result.Title = "Нехватка оперативной памяти (Out of Memory)";
                result.Summary = "Minecraft вылетел, потому что ему не хватило выделенной памяти для загрузки тяжелых модов.";
                result.Recommendation = "Увеличьте объем выделяемой памяти с помощью ползунка RAM на главном экране лаунчера (рекомендуется выделить 6–8 ГБ).";
            }
            else if (combined.IndexOf("ig7icd64.dll", StringComparison.OrdinalIgnoreCase) >= 0 ||
                     combined.IndexOf("ig9icd64.dll", StringComparison.OrdinalIgnoreCase) >= 0 ||
                     combined.IndexOf("nvoglv64.dll", StringComparison.OrdinalIgnoreCase) >= 0 ||
                     combined.IndexOf("atio6axx.dll", StringComparison.OrdinalIgnoreCase) >= 0 ||
                     combined.IndexOf("Pixel format not accelerated", StringComparison.OrdinalIgnoreCase) >= 0 ||
                     combined.IndexOf("GLFW error 65542", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                result.Title = "Сбой графического видеодрайвера или шейдеров";
                result.Summary = "Видеокарта или ее драйвер не смогли обработать текущий режим графики OpenGL.";
                result.Recommendation = "1. Переключите пресет графики на «Слабый ПК» на главном экране.\n2. Во вкладке «Настройки» выберите «Без шейдеров».\n3. Обновите драйвер видеокарты.";
            }
            else if (combined.IndexOf("UnsupportedClassVersionError", StringComparison.OrdinalIgnoreCase) >= 0 ||
                     combined.IndexOf("has been compiled by a more recent version", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                result.Title = "Несовместимая версия Java";
                result.Summary = "Для сборки Minecraft 1.21.1 с NeoForge требуется Java 21.";
                result.Recommendation = "Перейдите в «Настройки» и нажмите «Починить сборку» — лаунчер автоматически скачает чистую Java 21.";
            }
            else if (combined.IndexOf("ZipException", StringComparison.OrdinalIgnoreCase) >= 0 ||
                     combined.IndexOf("Invalid CEN header", StringComparison.OrdinalIgnoreCase) >= 0 ||
                     combined.IndexOf("Corrupt", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                result.Title = "Поврежден файл мода";
                result.Summary = "Один из jar-файлов модов был поврежден при загрузке или на диске.";
                result.Recommendation = "Нажмите «Починить сборку» в настройках лаунчера для автоматической перепроверки всех файлов.";
            }

            return result;
        }
    }

    public class CrashDoctorDialog : Form
    {
        private readonly CrashAnalysis _analysis;

        public CrashDoctorDialog(CrashAnalysis analysis)
        {
            _analysis = analysis ?? new CrashAnalysis();
            BuildUi();
        }

        private void BuildUi()
        {
            Text = "Доктор Steampunk & Swords — Анализ вылета";
            Size = new Size(620, 480);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            BackColor = Theme.Bg;
            ForeColor = Theme.Text;
            Font = Theme.Font(9.5f);

            var header = new Panel
            {
                Dock = DockStyle.Top,
                Height = 64,
                BackColor = Color.FromArgb(28, 18, 22) // Темно-бордовый аварийный
            };
            header.Paint += (s, e) =>
            {
                using (var pen = new Pen(Color.FromArgb(200, 220, 60, 60), 2))
                    e.Graphics.DrawLine(pen, 0, header.Height - 1, header.Width, header.Height - 1);
            };

            var lblHeader = new Label
            {
                Text = "⚠  " + _analysis.Title,
                Font = Theme.Display(12f, FontStyle.Bold),
                ForeColor = Color.FromArgb(255, 120, 120),
                Location = new Point(20, 18),
                AutoSize = true
            };
            header.Controls.Add(lblHeader);
            Controls.Add(header);

            var content = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(24, 16, 24, 16)
            };

            // Карточка проблемы
            var cardProblem = new Card
            {
                Location = new Point(24, 16),
                Size = new Size(556, 100)
            };
            var lblProbTitle = new Label
            {
                Text = "ЧТО ПРОИЗОШЛО:",
                Font = Theme.Font(8.5f, FontStyle.Bold),
                ForeColor = Theme.Gold,
                Location = new Point(14, 12),
                AutoSize = true
            };
            var lblProbDesc = new Label
            {
                Text = _analysis.Summary,
                Font = Theme.Font(9.5f),
                ForeColor = Theme.Text,
                Location = new Point(14, 34),
                Size = new Size(528, 56)
            };
            cardProblem.Controls.AddRange(new Control[] { lblProbTitle, lblProbDesc });
            content.Controls.Add(cardProblem);

            // Карточка решения
            var cardSolution = new Card
            {
                Location = new Point(24, 128),
                Size = new Size(556, 120)
            };
            var lblSolTitle = new Label
            {
                Text = "КАК ЭТО ИСПРАВИТЬ:",
                Font = Theme.Font(8.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(90, 210, 130), // Зеленый
                Location = new Point(14, 12),
                AutoSize = true
            };
            var lblSolDesc = new Label
            {
                Text = _analysis.Recommendation,
                Font = Theme.Font(9.5f),
                ForeColor = Theme.Text,
                Location = new Point(14, 34),
                Size = new Size(528, 76)
            };
            cardSolution.Controls.AddRange(new Control[] { lblSolTitle, lblSolDesc });
            content.Controls.Add(cardSolution);

            // Нижняя панель с кнопками
            var bottom = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 60,
                BackColor = Theme.Panel
            };

            var btnCopy = new AccentButton
            {
                Text = "📋 Скопировать лог для админа",
                Size = new Size(240, 38),
                Location = new Point(24, 11),
                Font = Theme.Font(9f, FontStyle.Bold)
            };
            btnCopy.Click += (s, e) =>
            {
                try
                {
                    var sb = new StringBuilder();
                    sb.AppendLine("=== Steampunk & Swords Crash Report ===");
                    sb.AppendLine("Диагноз: " + _analysis.Title);
                    sb.AppendLine("Описание: " + _analysis.Summary);
                    sb.AppendLine("Рекомендация: " + _analysis.Recommendation);
                    sb.AppendLine();
                    sb.AppendLine("=== Детали лога ===");
                    if (!string.IsNullOrEmpty(_analysis.Details))
                    {
                        int maxLen = Math.Min(4000, _analysis.Details.Length);
                        sb.AppendLine(_analysis.Details.Substring(_analysis.Details.Length - maxLen));
                    }
                    Clipboard.SetText(sb.ToString());
                    MessageBox.Show(this, "Отчет скопирован в буфер обмена!\nОтправьте его создателю сервера в Discord.",
                        "Скопировано", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, "Не удалось скопировать: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            var btnClose = new AccentButton
            {
                Text = "Понятно",
                Size = new Size(110, 38),
                Location = new Point(470, 11),
                Font = Theme.Font(9f, FontStyle.Bold),
                IsPrimary = true
            };
            btnClose.SetGoldGlowStyle();
            btnClose.Click += (s, e) => Close();

            bottom.Controls.AddRange(new Control[] { btnCopy, btnClose });
            Controls.Add(content);
            Controls.Add(bottom);
        }
    }
}
