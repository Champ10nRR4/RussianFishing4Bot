using Emgu.CV.Face;
using Emgu.CV.OCR;
using System.Collections;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using TesseractOCR;
using TesseractOCR.Enums;
using WindowsInput;
using WindowsInput.Native;
using static System.Runtime.CompilerServices.RuntimeHelpers;


namespace KeyPresser
{
    public partial class Form1 : Form
    {
        #region Imports

        [DllImport("user32.dll")]
        static extern bool GetCursorPos(ref Point lpPoint);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        public static extern int BitBlt(IntPtr hDC, int x, int y, int nWidth, int nHeight, IntPtr hSrcDC, int xSrc, int ySrc, int dwRop);

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool GetWindowRect(IntPtr hWnd, out Structs.RECT lpRect);

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool GetClientRect(IntPtr hWnd, out Structs.RECT lpRect);

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool GetWindowPlacement(IntPtr hWnd, out Structs.WINDOWPLACEMENT lpWndpl);

        #endregion

        private readonly string basePath = AppDomain.CurrentDomain.BaseDirectory;
        private readonly AppConfiguration configuration;
        private string dataDirectory;
        private Status currentStatus;
        KeyboardHook hook = new KeyboardHook();
        private int skipMin;
        private int skipMax;
        private int holdMin;
        private int holdMax;
        private CancellationTokenSource cancellationTokenSource;
        private Point cursor = new Point();
        private static System.Timers.Timer pickColourTimer = new System.Timers.Timer();
        private static System.Timers.Timer monitorBaitTimer = new System.Timers.Timer();
        private static System.Timers.Timer readyToCastTimer = new System.Timers.Timer();
        private static System.Timers.Timer fishOnDisplayMonitor = new System.Timers.Timer();
        private static System.Timers.Timer awaitBaitToDrop = new System.Timers.Timer();
        private static System.Timers.Timer errorMonitorTimer = new System.Timers.Timer();

        Bitmap screenPixel = new Bitmap(1, 1, PixelFormat.Format32bppArgb);
        //private bool monitoringBite;
        private bool keepRetrieval;
        private int reelRetrievalCounter;
        private int reelRetrievalRoundsCounter;
        private IDataServices dataServices;

        private LocationObject bonusColorPoint;
        private LocationObject fishIconPoint;
        private LocationObject errorMessagePoint;
        private LocationObject energyPoint;
        private LocationObject foodPoint;

        private LocationObject craftButton;
        private LocationObject craftedOkButton;
        private LocationObject errorButton;
        private LocationObject fishToTheNetButton;
        private LocationObject releaseFishButton;



        private int screenWidth;
        private int screenHeight;

        private int windowLeft;
        private int windowTop;
        private int windowWidth;
        private int windowHeight;
        private int windowRight;
        private int windowBottom;
        private int windowCenterX;
        private int windowCenterY;

        private string WindowName;

        public Form1(AppConfiguration configuration)
        {
            this.configuration = configuration;
            InitializeComponent();
            Init();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.dataDirectory = basePath + "Data\\";
            dataServices = new DataServices(dataDirectory);
            var profile = dataServices.GetProfile(0);
            if (profile != null)
            {
                tbCoordX.Text = profile.ColourPickerX.ToString();
                tbCoordY.Text = profile.ColourPickerY.ToString();
                tbHoldMax.Text = profile.ActionMax.ToString();
                tbHoldMin.Text = profile.ActionMin.ToString();
                tbActionDelay.Text = profile.ActionDelay.ToString();
                tbSkipMax.Text = profile.PauseMax.ToString();
                tbSkipMin.Text = profile.PauseMin.ToString();
                rbPull.Checked = profile.ActionType == ActionType.Pull;
                rbSpin.Checked = profile.ActionType == ActionType.Spin;
                cbSeaFishing.Checked = profile.SeaFishing;
                cbFloat.Checked = profile.IsFloat;
                cbAuto.Checked = profile.RiseRod;
            }
        }

        private void Init()
        {
            tbSkipMin.Text = configuration.AppDefaults.WaitMinMs.ToString();
            tbSkipMax.Text = configuration.AppDefaults.WaitMaxMs.ToString();
            tbHoldMin.Text = configuration.AppDefaults.HoldMinMs.ToString();
            tbHoldMax.Text = configuration.AppDefaults.HoldMaxMs.ToString();
            tbActionDelay.Text = configuration.AppDefaults.ActionDelayS.ToString();
            rbSpin.Select();
            cancellationTokenSource = new CancellationTokenSource();

            hook.KeyPressed +=
            new EventHandler<KeyPressedEventArgs>(hook_KeyPressed);

            hook.RegisterHotKey(KeyPresser.ModifierKeys.Control | KeyPresser.ModifierKeys.Alt, Keys.S);
            hook.RegisterHotKey(KeyPresser.ModifierKeys.Control | KeyPresser.ModifierKeys.Alt, Keys.W);
            hook.RegisterHotKey(KeyPresser.ModifierKeys.Control | KeyPresser.ModifierKeys.Alt, Keys.R);

            GetWindowCoord(configuration.AppDefaults.WindowName);
            GetPoints();

            monitorBaitTimer.Interval = configuration.Intervals.MonitorColourTimerMs;
            monitorBaitTimer.Elapsed += MonitorBaitTimer_Elapsed;

            readyToCastTimer.Interval = configuration.Intervals.ReadyToCastTimerMs;
            readyToCastTimer.Elapsed += ReadyToCastTimer_Elapsed;

            fishOnDisplayMonitor.Interval = configuration.Intervals.FishOnDisplayMonitorMs;
            fishOnDisplayMonitor.Elapsed += CheckFishOnDisplayTimer_Elapsed;

            awaitBaitToDrop.AutoReset = false;
            awaitBaitToDrop.Elapsed += AwaitBaitToDropTimer_Elapsed;

            pickColourTimer.Interval = configuration.Intervals.PickColourTimerMs;
            pickColourTimer.AutoReset = false;
            pickColourTimer.Elapsed += PickColourTimer_Elapsed;

            errorMonitorTimer.Interval = configuration.Intervals.ErrorMonitorTimerMs;
            pickColourTimer.Elapsed += ErrorMonitorTimer_Elapsed;
        }

        private async void btnStart_Click(object sender, EventArgs e)
        {
            StartProcess(cancellationTokenSource.Token);
        }

        private async void btnStop_Click(object sender, EventArgs e)
        {
            StopProcess(true);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void pickCoordinatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            monitorBaitTimer.Stop();
            StopProcess(false);
            LogToListbox("Finding colour");
            pickColourTimer.Start();
        }

        private void initializeCoordinatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GetPoints();
        }

        private void configurationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveProfile();
        }

        private void OptionsToStatus()
        {
            if (cbSeaFishing.Checked)
            {
                currentStatus = Status.Pilking;
            }
            else if (cbFloat.Checked)
            {
                currentStatus = Status.FloatFishing;
            }
            else if (cbDig.Checked)
            {
                currentStatus = Status.Digging;
            }
            else if (cbBrewing.Checked)
            {
                currentStatus = Status.Crafting;
            }
            else if (cbSpinning.Checked)
            {
                currentStatus = Status.Spinning;
            }
            else if (cbJigging.Checked)
            {
                currentStatus = Status.Jigging;
            }
            else
            {
                currentStatus = Status.Idle;
            }
            lblStatus.Text = currentStatus.ToString();
            LogToListbox($"Status changed to {currentStatus.ToString()}");
        }

        private void StartProcess(CancellationToken token)
        {
            LogToListbox($"Process started");
            try
            {
                skipMin = int.Parse(tbSkipMin.Text);
                skipMax = int.Parse(tbSkipMax.Text);
                holdMin = int.Parse(tbHoldMin.Text);
                holdMax = int.Parse(tbHoldMax.Text);
                listBox1.Items.Clear();
                keepRetrieval = false;

                this.BackColor = DefaultBackColor;
                LogToListbox("Process started");

                OptionsToStatus();
                if (!monitorBaitTimer.Enabled && currentStatus != Status.Digging && currentStatus != Status.Digging)
                {
                    monitorBaitTimer.Start();
                }

                var task = Task.Factory.StartNew(async () =>
                {
                    await StartAction(token);
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void StopProcess(bool stopAllTimers)
        {
            InputSimulator simulator = new InputSimulator();
            simulator.Keyboard.KeyUp(VirtualKeyCode.NUMPAD0);
            simulator.Keyboard.KeyUp(VirtualKeyCode.DIVIDE);
            simulator.Keyboard.KeyUp(VirtualKeyCode.DECIMAL);
            simulator.Mouse.LeftButtonUp();
            simulator.Mouse.RightButtonUp();

            cancellationTokenSource.Cancel();
            currentStatus = Status.Idle;
            lblStatus.Text = currentStatus.ToString();
            LogToListbox($"Status changed to {currentStatus.ToString()}");
            keepRetrieval = false;
            pickColourTimer.Stop();
            monitorBaitTimer.Stop();
            awaitBaitToDrop.Stop();
            if (stopAllTimers)
            {
                awaitBaitToDrop.Stop();
                fishOnDisplayMonitor.Stop();
            }
            this.BackColor = DefaultBackColor;
            LogToListbox("Process stopped");
        }

        private async Task StartAction(CancellationToken token)
        {
            var spinPressed = false;
            InputSimulator simulator = new InputSimulator();
            var waitRandom = new Random();
            var holdRandom = new Random();
            var cycleRandom = new Random();
            var timesRandom = new Random();
            var keyCode = rbSpin.Checked ? VirtualKeyCode.NUMPAD0 : VirtualKeyCode.DIVIDE;
            var cycleCount = 100;

            if (currentStatus == Status.Digging)
            {
                cycleCount = 10000;
            }
            if (currentStatus == Status.Spinning)
            {
                cycleCount = 1;
            }

            for (int i = 0; i < cycleCount; i++)
            {
                var actingStatus = currentStatus == Status.Jigging || currentStatus == Status.Spinning || currentStatus == Status.Pilking || currentStatus == Status.FloatFishing || currentStatus == Status.Digging || currentStatus == Status.Crafting;
                if (actingStatus && !token.IsCancellationRequested)
                {
                    try
                    {
                        var skip = waitRandom.Next(skipMin, skipMax);
                        var hold = holdRandom.Next(holdMin, holdMax);

                        LogToListbox("pause(ms): " + skip);
                        await Task.Delay(skip, token);

                        if (actingStatus && !token.IsCancellationRequested)
                        {
                            LogToListbox("action (ms): " + +hold);
                            if (currentStatus == Status.Digging)
                            {
                                DigBite();
                            }
                            else if (currentStatus == Status.Crafting)
                            {
                                Craft();
                            }
                            else if (currentStatus == Status.Spinning)
                            {
                                if (!spinPressed) { 
                                    simulator.Keyboard.KeyDown(keyCode);
                                    spinPressed = true;
                                }
                            }
                            else if (currentStatus == Status.Jigging || currentStatus == Status.Pilking || currentStatus == Status.FloatFishing)
                            {
                                simulator.Keyboard.KeyDown(keyCode);
                                simulator.Keyboard.Sleep(hold);
                                simulator.Keyboard.KeyUp(keyCode);
                            }
                        }
                    }
                    catch (TaskCanceledException tcex)
                    {
                        LogToListbox("Process ended due to exception");
                        return;
                    }
                }
                else
                {
                    if (keepRetrieval)
                    {
                        LogToListbox("Keep Retrieving");
                        RetrieveReel();
                    }
                    this.BackColor = DefaultBackColor;
                    LogToListbox("Process cycle exit");
                    return;
                }
            }

            LogToListbox("Process ended");
        }

        void hook_KeyPressed(object sender, KeyPressedEventArgs e)
        {
            if (e.Key == Keys.R)
            {
                RetrieveReel();
            }
            else if (e.Key == Keys.S)
            {
                if (currentStatus != Status.Idle)
                {
                    StopProcess(true);
                }
                else
                {
                    using (var cancellationTokenSource = new CancellationTokenSource())
                    {
                        StartProcess(cancellationTokenSource.Token);
                    }
                }
            }
            else if (e.Key == Keys.W)
            {
                if (cbSeaFishing.Checked)
                {
                    var task = Task.Factory.StartNew(async () => { await SeaCastAndMonitor(); });
                }
                else if (cbSpinning.Checked || cbJigging.Checked)
                {
                    var task = Task.Factory.StartNew(async () => { await SpinningCastAndMonitor(false); });
                }
                else if (cbFloat.Checked)
                {
                    var task = Task.Factory.StartNew(async () => { await FloatCastAndMonitor(); });
                }
            }
        }

        private void ErrorMonitorTimer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            if (IsErrorDisplayed())
            {
                StopProcess(true);
            }
        }

        private void PickColourTimer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            GetCursorPos(ref cursor);
            LogToListbox("Coordinates: " + cursor.X + ":" + cursor.Y);
            tbCoordX.Text = cursor.X.ToString();
            tbCoordY.Text = cursor.Y.ToString();
        }

        private void MonitorBaitTimer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            if (currentStatus != Status.Idle) { 
                MonitoringBite();
            }
        }

        private void ReadyToCastTimer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            if (currentStatus == Status.Spinning || currentStatus == Status.Jigging)
            {
                MonitoringReadyToCast();
            }
        }

        private void CheckFishOnDisplayTimer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            if (currentStatus != Status.Idle) {
                {
                    CheckFishOnDisplay();
                }
            }
        }

        private void AwaitBaitToDropTimer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            if (currentStatus == Status.AwaitingBiteToDrop)
            {
                LogToListbox($"Action delay elapsed");
                InputSimulator simulator = new InputSimulator();
                var num0Key = VirtualKeyCode.NUMPAD0;
                simulator.Keyboard.KeyPress(num0Key);
                LogToListbox($"Starting action");

                using (var cancellationTokenSource = new CancellationTokenSource())
                {
                    StartProcess(cancellationTokenSource.Token);
                }
            }
        }

        private void AwaitFloatTimer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            if (currentStatus == Status.AwaitingFloat)
            {
                LogToListbox($"Action delay elapsed");
                LogToListbox($"Starting action");
                using (var cancellationTokenSource = new CancellationTokenSource())
                {
                    StartProcess(cancellationTokenSource.Token);
                }
            }
        }

        private void CheckFishOnDisplay()
        {
            InputSimulator simulator = new InputSimulator();
            var keyCode1 = VirtualKeyCode.SPACE;
            var keyCode2 = VirtualKeyCode.DIVIDE;
            var randomX = new Random();
            var randomY = new Random();



            var collectFish = false;
            if (reelRetrievalCounter <= 5)
            {
                if (currentStatus == Status.AwaitingFishToDisplay)
                {
                    if (IsFishDisplayed())
                    {
                        collectFish = true;
                    }
                }
            }
            else
            {
                reelRetrievalRoundsCounter++;
                if (IsFishOnHook())
                {
                    LogToListbox("Fish still on the hook");
                    reelRetrievalCounter = 0;
                    if (cbFloat.Checked)
                    {
                        simulator.Keyboard.KeyPress(keyCode1);
                    }
                    if (reelRetrievalCounter % 5 == 0)
                    {
                        simulator.Keyboard.Sleep((new Random()).Next(300, 800));
                        simulator.Keyboard.KeyPress(keyCode2);
                        simulator.Keyboard.Sleep((new Random()).Next(100, 300));
                        simulator.Keyboard.KeyDown(keyCode2);
                    }
                }
                else
                {
                    if (reelRetrievalCounter > 5 && reelRetrievalCounter <= 8)
                    {
                        if (IsFishDisplayed())
                        {
                            LogToListbox("Fish finally displayed");
                            reelRetrievalRoundsCounter = 0;
                            collectFish = true;
                        }
                        else
                        {
                            LogToListbox("Last chances to detect fish");
                        }
                    }
                    else
                    {
                        LogToListbox("Could not decect where the fish is");
                        fishOnDisplayMonitor.Stop();
                        OptionsToStatus();
                        reelRetrievalRoundsCounter = 0;
                        if (!IsFishDisplayed())
                        {
                            if (cbSeaFishing.Checked)
                            {
                                var task = Task.Factory.StartNew(async () => { await SeaCastAndMonitor(); });
                            }
                            else if (cbSpinning.Checked || cbJigging.Checked)
                            {
                                var task = Task.Factory.StartNew(async () => { await SpinningCastAndMonitor(); });
                            }
                            else if (cbFloat.Checked)
                            {

                                var task = Task.Factory.StartNew(async () => { await FloatCastAndMonitor(); });
                            }
                        }
                    }
                }
            }

            if (collectFish)
            {
                currentStatus = Status.CollectingFish;
                lblStatus.Text = currentStatus.ToString();
                LogToListbox($"Status changed to {currentStatus.ToString()}");
                LogToListbox("Fish is out of the water ");
                var hold1Random = new Random();
                var hold2Random = new Random();
                var hold1 = hold1Random.Next(1000, 2000);
                var hold2 = hold2Random.Next(1000, 2000);
                simulator.Keyboard.Sleep(hold1);
                var mousex = (double)0;
                var mousey = (double)0;

                if (IsFishValid())
                {

                    LogToListbox("Accountable fish, collecting");
                    var fishToNetX = randomX.Next(fishToTheNetButton.CoordinateX, fishToTheNetButton.CoordinateX + fishToTheNetButton.Width);
                    var fishToNetY = randomY.Next(fishToTheNetButton.CoordinateY, fishToTheNetButton.CoordinateY + fishToTheNetButton.Height);
                    mousex = (double)65535 * fishToNetX / screenWidth;
                    mousey = (double)65535 * fishToNetY / screenHeight;
                }
                else
                {
                    LogToListbox("Not accountable fish, throwing away");
                    var releaseFishX = randomX.Next(releaseFishButton.CoordinateX, releaseFishButton.CoordinateX + releaseFishButton.Width);
                    var releaseFishY = randomY.Next(releaseFishButton.CoordinateY, releaseFishButton.CoordinateY + releaseFishButton.Height);
                    mousex = (double)65535 * releaseFishX / screenWidth;
                    mousey = (double)65535 * releaseFishY / screenHeight;
                }
                simulator.Mouse.MoveMouseTo(mousex, mousey);
                simulator.Mouse.LeftButtonClick();

                simulator.Mouse.Sleep(500);
                var errorDisplayed = IsErrorDisplayed();
                simulator.Mouse.Sleep(hold2);
                fishOnDisplayMonitor.Stop();

                if (errorDisplayed)
                {
                    currentStatus = Status.Idle;
                    lblStatus.Text = currentStatus.ToString();
                    LogToListbox($"Status changed to {currentStatus.ToString()}");
                    LogToListbox("Fish net is full");
                }
                else
                {
                    if (cbSeaFishing.Checked)
                    {
                        LogToListbox("Continue sea fishing");
                        var task = Task.Factory.StartNew(async () => { await SeaCastAndMonitor(); });
                    }
                    else if (cbSpinning.Checked || cbJigging.Checked)
                    {
                        LogToListbox("Continue spinning fishing");
                        var task = Task.Factory.StartNew(async () => { await SpinningCastAndMonitor(); });
                    }
                    else if (cbFloat.Checked)
                    {
                        LogToListbox("Continue float fishing");
                        var task = Task.Factory.StartNew(async () => { await FloatCastAndMonitor(); });
                    }
                }
            }

            reelRetrievalCounter++;
        }

        private void MonitoringBite()
        {
            var monitoringBite = 
                    currentStatus == Status.Pilking || 
                    currentStatus == Status.Jigging || 
                    currentStatus == Status.Spinning || 
                    currentStatus == Status.FloatFishing || 
                    currentStatus == Status.AwaitingBiteToDrop;
            if (monitoringBite)
            {
                if (currentStatus == Status.FloatFishing)
                {
                    var cur = new Point(int.Parse(tbCoordX.Text), int.Parse(tbCoordY.Text));
                    var c = GetColorAt(cur);
                    pnlColourIndicator.BackColor = c;

                    if (ConfigToColour(c, "FloatBaitColour"))
                    {
                        LogToListbox("2 sec delay");
                        monitorBaitTimer.Stop();
                        Thread.Sleep(2000);
                        monitorBaitTimer.Start();
                        c = GetColorAt(cur);
                        if (ConfigToColour(c, "FloatBaitColour"))
                        {
                            LogToListbox("lifting the rod");
                            pnlColourIndicator.BackColor = c;
                            //this.BackColor = c;
                            monitorBaitTimer.Stop();
                            keepRetrieval = true;
                            RetrieveRod();
                        }
                        else
                        {
                            LogToListbox("can see the float");
                            LogToListbox($"R:{c.R} G:{c.G} B:{c.B}");
                        }
                    }
                }
                else if ((currentStatus == Status.Pilking || currentStatus == Status.Jigging || currentStatus == Status.Spinning || currentStatus == Status.AwaitingBiteToDrop) && IsFishOnHook())
                {
                    this.BackColor = Color.FromArgb(50, 125, 180);
                    monitorBaitTimer.Stop();

                    StopProcess(false);
                    LogToListbox("Speening the reel");
                    keepRetrieval = true;
                    RetrieveReel();
                }

            }
        }

        private void MonitoringReadyToCast()
        {
            var monitoringRetrieval = currentStatus == Status.Jigging || currentStatus == Status.Spinning;
            if (monitoringRetrieval) { 
                if (IsReadyToCast())
                {
                    var task = Task.Factory.StartNew(async () => { await SpinningCastAndMonitor(); });
                }
            }
        }

        private void PullTheFish(bool startMonitoring)
        {
            if (startMonitoring)
            {
                currentStatus = Status.AwaitingFishToDisplay;
                lblStatus.Text = currentStatus.ToString();
                LogToListbox($"Status changed to {currentStatus.ToString()}");
                reelRetrievalCounter = 0;
                LogToListbox($"Start monitoring fish out of water colour");
                fishOnDisplayMonitor.Start();
                awaitBaitToDrop.Stop();
                monitorBaitTimer.Stop();
            }
        }

        private void RetrieveReel()
        {
            {
                RetrieveRFReel();
            }
            
        }

        private void RetrieveRFReel() {
            currentStatus = Status.RetievingTheReel;
            lblStatus.Text = currentStatus.ToString();
            LogToListbox($"Status changed to {currentStatus.ToString()}");
            InputSimulator simulator = new InputSimulator();
            var randomPause = new Random();
            var num0Key = VirtualKeyCode.NUMPAD0;
            var decimalKey = VirtualKeyCode.DECIMAL;
            var divideKey = VirtualKeyCode.DIVIDE;

            var num0PausePress = randomPause.Next(200, 500);
            var decimalPausePress = randomPause.Next(2000, 5000);

            simulator.Keyboard.KeyDown(num0Key);
            simulator.Keyboard.Sleep(num0PausePress);

            simulator.Keyboard.KeyDown(decimalKey);
            simulator.Keyboard.Sleep(decimalPausePress);

            if (cbAuto.Checked)
            {
                simulator.Keyboard.KeyDown(divideKey);
                PullTheFish(true);
            }
        }

        private void RetrieveRod()
        {
            currentStatus = Status.RetievingTheRod;
            lblStatus.Text = currentStatus.ToString();
            LogToListbox($"Status changed to {currentStatus.ToString()}");
            InputSimulator simulator = new InputSimulator();
            var randomPause = new Random();
            var num0Key = VirtualKeyCode.NUMPAD0;

            var num0PausePress = randomPause.Next(200, 500);
            var decimalPausePress = randomPause.Next(2000, 5000);

            simulator.Keyboard.KeyDown(num0Key);
            simulator.Keyboard.Sleep(num0PausePress);

            if (cbAuto.Checked)
            {
                simulator.Keyboard.KeyDown(num0Key);
                PullTheFish(true);
            }
        }

        private void Craft()
        {
            InputSimulator simulator = new InputSimulator();
            var randomX = new Random();
            var randomY = new Random();
            var waitRandom = new Random();
            var waitMs = waitRandom.Next(4500, 8000);

            try
            {
                var crdx = randomX.Next(craftButton.CoordinateX, craftButton.CoordinateX + craftButton.Width);
                var crdY = randomY.Next(craftButton.CoordinateY, craftButton.CoordinateY + craftButton.Height);
                var mousex = (double)65535 * crdx / screenWidth;
                var mousey = (double)65535 * crdY / screenHeight;
                simulator.Mouse.MoveMouseTo(mousex, mousey);
                simulator.Mouse.LeftButtonClick();
                simulator.Mouse.Sleep(waitMs);

                if (IsCraftErrorShow())
                {
                    var errx = randomX.Next(errorButton.CoordinateX, errorButton.CoordinateX + errorButton.Width);
                    var errY = randomY.Next(errorButton.CoordinateY, errorButton.CoordinateY + errorButton.Height);
                    mousex = (double)65535 * errx / screenWidth;
                    mousey = (double)65535 * errY / screenHeight;
                    simulator.Mouse.MoveMouseTo(mousex, mousey);
                }
                else
                {
                    var creaftedX = randomX.Next(craftedOkButton.CoordinateX, craftedOkButton.CoordinateX + craftedOkButton.Width);
                    var craftedY = randomY.Next(craftedOkButton.CoordinateY, craftedOkButton.CoordinateY + craftedOkButton.Height);
                    mousex = (double)65535 * creaftedX / screenWidth;
                    mousey = (double)65535 * craftedY / screenHeight;
                    simulator.Mouse.MoveMouseTo(mousex, mousey);
                }
                simulator.Mouse.LeftButtonClick();
            }
            catch (Exception ex)
            {
                LogToListbox($"{ex.Message}");
            }
        }

        private void DigBite()
        {
            InputSimulator simulator = new InputSimulator();
            var waitRandom = new Random();
            var waitdiggingRandom = new Random();

            var digWaiting = waitRandom.Next(configuration.Intervals.DiggingWaitMinMs, configuration.Intervals.DiggingWaitMaxMs);
            var snackWaiting = waitdiggingRandom.Next(configuration.Intervals.SnackingWaitMinMs, configuration.Intervals.SnackingWaitMaxMs);
            var spaceButton = VirtualKeyCode.SPACE;
            var snackButton = VirtualKeyCode.NUMPAD5;

            if (IsEnergyReplenished())
            {
                LogToListbox($"full energy. Digging");
                simulator.Mouse.LeftButtonClick();
                simulator.Keyboard.Sleep(digWaiting);
                simulator.Keyboard.KeyPress(spaceButton);
            }

            simulator.Keyboard.Sleep(snackWaiting);
            if (!IsFishermanFull())
            {
                LogToListbox($"Hungry. Snacking");
                simulator.Keyboard.KeyPress(snackButton);
            }
        }

        private async Task SeaCastAndMonitor()
        {
            currentStatus = Status.ResettingAutoFishing;
            LogToListbox($"Status changed to {currentStatus.ToString()}");
            lblStatus.Text = currentStatus.ToString();
            InputSimulator simulator = new InputSimulator();
            var hold1Random = new Random();
            var hold2Random = new Random();
            var hold3Random = new Random();
            var hold4Random = new Random();
            var keyCode1 = VirtualKeyCode.NUMPAD0;
            var keyCode2 = VirtualKeyCode.DECIMAL;
            var keyCode3 = VirtualKeyCode.DIVIDE;

            var hold1 = hold1Random.Next(100, 300);
            var hold2 = hold2Random.Next(200, 400);
            var hold3 = hold3Random.Next(50, 200);
            var hold4 = hold4Random.Next(300, 600);

            //MonitorColour(!monitoringBite);
            simulator.Keyboard.Sleep(hold1);
            simulator.Keyboard.KeyPress(keyCode2);
            simulator.Keyboard.Sleep(hold2);
            simulator.Keyboard.KeyPress(keyCode1);
            simulator.Keyboard.Sleep(hold3);
            simulator.Keyboard.KeyPress(keyCode1);

            if (cbAuto.Checked)
            {
                simulator.Keyboard.Sleep(hold4);
                simulator.Keyboard.KeyPress(keyCode3);
                if (!string.IsNullOrEmpty(this.tbActionDelay.Text))
                {
                    currentStatus = Status.AwaitingBiteToDrop;
                    LogToListbox($"Status changed to {currentStatus.ToString()}");
                    lblStatus.Text = currentStatus.ToString();
                    var delay = int.Parse(this.tbActionDelay.Text) * 1000;
                    var randomPause = new Random();
                    var num0PausePress = randomPause.Next(delay, delay + 3000);
                    LogToListbox($"Triggered delay of: {tbActionDelay.Text} seconds");
                    awaitBaitToDrop.Interval = num0PausePress;
                    awaitBaitToDrop.Start();
                    monitorBaitTimer.Start();
                }
            }
        }

        private async Task SpinningCastAndMonitor(bool needResetButtons = true)
        {
            currentStatus = Status.ResettingAutoFishing;
            LogToListbox($"Status changed to {currentStatus.ToString()}");
            lblStatus.Text = currentStatus.ToString();
            InputSimulator simulator = new InputSimulator();
            var hold1Random = new Random();
            var hold2Random = new Random();
            var hold3Random = new Random();
            var hold4Random = new Random();
            var hold5Random = new Random();
            var keyCode1 = VirtualKeyCode.NUMPAD0;
            var keyCode2 = VirtualKeyCode.DECIMAL;
            var keyCode3 = VirtualKeyCode.DIVIDE;

            var hold1 = hold1Random.Next(100, 300);
            var hold2 = hold2Random.Next(200, 400);
            var hold3 = hold3Random.Next(50, 200);
            var hold4 = hold4Random.Next(300, 600);
            var hold5 = hold4Random.Next(2500, 3500);

            if (needResetButtons) { 
                simulator.Keyboard.Sleep(hold1);
                simulator.Keyboard.KeyPress(keyCode2);
                simulator.Keyboard.Sleep(hold2);
                simulator.Keyboard.KeyPress(keyCode1);
            }


            if (cbAuto.Checked)
            {
                LogToListbox($"Casting spinning");
                simulator.Keyboard.Sleep(hold4);
                simulator.Keyboard.KeyDown(keyCode1);
                simulator.Keyboard.Sleep(hold5);
                simulator.Keyboard.KeyUp(keyCode1);
                if (!string.IsNullOrEmpty(this.tbActionDelay.Text))
                {
                    currentStatus = Status.AwaitingBiteToDrop;
                    LogToListbox($"Status changed to {currentStatus.ToString()}");
                    lblStatus.Text = currentStatus.ToString();
                    var delay = int.Parse(this.tbActionDelay.Text) * 1000;
                    var randomPause = new Random();
                    var num0PausePress = randomPause.Next(delay, delay + 3000);
                    LogToListbox($"Triggered delay of: {tbActionDelay.Text} seconds");
                    awaitBaitToDrop.Interval = num0PausePress;
                    awaitBaitToDrop.Start();
                    monitorBaitTimer.Start();
                    readyToCastTimer.Start();
                }
            }
        }

        private async Task FloatCastAndMonitor()
        {
            currentStatus = Status.ResettingAutoFishing;
            LogToListbox($"Status changed to {currentStatus.ToString()}");
            lblStatus.Text = currentStatus.ToString();
            InputSimulator simulator = new InputSimulator();
            var hold1Random = new Random();
            var hold2Random = new Random();
            var hold4Random = new Random();
            var keyCode1 = VirtualKeyCode.NUMPAD0;
            holdMin = int.Parse(tbHoldMin.Text);
            holdMax = int.Parse(tbHoldMax.Text);

            var delay = int.Parse(this.tbActionDelay.Text) * 1000;

            var hold1 = hold1Random.Next(100, 300);
            var hold2 = hold2Random.Next(holdMin, holdMax);
            var hold4 = hold4Random.Next(100, 300);

            monitorBaitTimer.Stop();
            simulator.Mouse.Sleep(hold1);
            simulator.Mouse.LeftButtonDown();
            LogToListbox("Casting " + hold2.ToString());
            simulator.Mouse.Sleep(hold2);
            simulator.Mouse.LeftButtonUp();
            simulator.Mouse.Sleep(hold4);
            simulator.Keyboard.KeyPress(keyCode1);

            LogToListbox($"Holding {(delay / 1000).ToString()} seconds");
            currentStatus = Status.AwaitingFloat;
            LogToListbox($"Status changed to {currentStatus.ToString()}");
            lblStatus.Text = currentStatus.ToString();
            simulator.Mouse.Sleep(delay);

            currentStatus = Status.FloatFishing;
            LogToListbox($"Status changed to {currentStatus.ToString()}");
            lblStatus.Text = currentStatus.ToString();
            if (!monitorBaitTimer.Enabled)
            {
                monitorBaitTimer.Start();
            }
        }

        private void SaveProfile()
        {
            var profile = new Profile()
            {
                ColourPickerX = int.Parse(tbCoordX.Text),
                ColourPickerY = int.Parse(tbCoordY.Text),
                ActionMax = int.Parse(tbHoldMax.Text),
                ActionMin = int.Parse(tbHoldMin.Text),
                ActionDelay = int.Parse(tbActionDelay.Text),
                PauseMax = int.Parse(tbSkipMax.Text),
                PauseMin = int.Parse(tbSkipMin.Text),
                ActionType = rbPull.Checked ? ActionType.Pull : ActionType.Spin,
                SeaFishing = cbSeaFishing.Checked,
                IsFloat = cbFloat.Checked,
                RiseRod = cbAuto.Checked
            };

            dataServices.SaveProfile(profile);
        }

        private bool IsFishDisplayed()
        {
            var result = false;
            var imageBytes = CaptureScreenHelper.CaptureScreen(windowLeft, windowBottom - 400, windowWidth, 250);
            //File.WriteAllBytes("D:\\Temporary\\test.png", imageBytes);
            var detector = new TemplateMatchDetector();
            result = detector.DetectValid(imageBytes, "xp.png");

            return result;

            //LogToListbox("Check fish out of the water try " + reelRetrievalCounter);
            //var c = GetColorAt(new Point(bonusColorPoint.CoordinateX, bonusColorPoint.CoordinateY));
            //var cBonusHour = GetColorAt(new Point(bonusColorPoint.CoordinateX - 85, bonusColorPoint.CoordinateY));
            //pnlColourIndicator.BackColor = c;
            //return ConfigToColour(c, "FishDisplayedColour") || ConfigToColour(cBonusHour, "FishDisplayedColour");
        }

        private bool IsErrorDisplayed()
        {
            LogToListbox("Check error is displayed");
            var c = GetColorAt(new Point(errorMessagePoint.CoordinateX, errorMessagePoint.CoordinateY));
            pnlColourIndicator.BackColor = c;
            return ConfigToColour(c, "ErrorDisplayedColour");
        }

        private bool IsFishOnHook()
        {
            var cFishIcon = GetColorAt(new Point(fishIconPoint.CoordinateX, fishIconPoint.CoordinateY));
            pnlColourIndicator.BackColor = cFishIcon;

            return ConfigToColour(cFishIcon, "FishOnHookColour");
        }

        private bool IsEnergyReplenished()
        {
            var energyColour = GetColorAt(new Point(energyPoint.CoordinateX, energyPoint.CoordinateY));
            pnlColourIndicator.BackColor = energyColour;

            return ConfigToColour(energyColour, "EnergyReplenishedColour");
        }

        private bool IsFishermanFull()
        {
            var foodColour = GetColorAt(new Point(foodPoint.CoordinateX, foodPoint.CoordinateY));
            pnlColourIndicator.BackColor = foodColour;
            return ConfigToColour(foodColour, "FishermanFullColour");

        }
        private bool IsCraftErrorShow()
        {
            var c = GetColorAt(new Point(errorButton.CoordinateX + errorButton.Width / 2, errorButton.CoordinateY + errorButton.Height / 2));
            pnlColourIndicator.BackColor = c;

            return ConfigToColour(c, "CraftErrorShowColour");
        }

        private void GetWindowCoord(string windowName)
        {
            IntPtr hWnd = FindWindow(null, windowName);
            if (hWnd != IntPtr.Zero)
            {
                Structs.RECT windowPosition;
                GetWindowRect(hWnd, out windowPosition);
                windowLeft = windowPosition.Left;
                windowTop = windowPosition.Top;
                windowRight = windowPosition.Right;
                windowBottom = windowPosition.Bottom;
                windowWidth = windowRight - windowLeft;
                windowHeight = windowBottom - windowTop;
                windowCenterX = windowLeft + windowWidth / 2;
                windowCenterY = windowTop + windowHeight / 2;
            }
        }

        private void GetPoints()
        {
            screenWidth = Screen.PrimaryScreen.Bounds.Width;
            screenHeight = Screen.PrimaryScreen.Bounds.Height;

            bonusColorPoint = ConfigToCoordinates("BonusColorPoint");
            LogToListbox($"Getting bonus colour coord: X:{bonusColorPoint.CoordinateX} Y:{bonusColorPoint.CoordinateY}");

            fishIconPoint = ConfigToCoordinates("FishIconPoint");
            LogToListbox($"Getting fish icon : X:{fishIconPoint.CoordinateX} Y:{fishIconPoint.CoordinateY}");

            foodPoint = ConfigToCoordinates("FoodPoint");
            LogToListbox($"Food coord: X:{foodPoint.CoordinateX} Y:{foodPoint.CoordinateY}");

            energyPoint = ConfigToCoordinates("EnergyPoint");
            LogToListbox($"Energy coord: X:{energyPoint.CoordinateX} Y:{energyPoint.CoordinateY}");

            errorMessagePoint = ConfigToCoordinates("ErrorMessagePoint");
            LogToListbox($"Getting errpr button coord: X:{errorMessagePoint.CoordinateX} Y:{errorMessagePoint.CoordinateY}");

            errorButton = ConfigToCoordinates("ErrorButton");
            LogToListbox($"Getting errpr button coord: X:{errorButton.CoordinateX} Y:{errorButton.CoordinateY}");

            fishToTheNetButton = ConfigToCoordinates("FishToTheNetButton");
            LogToListbox($"Fish To Net button coord: X:{fishToTheNetButton.CoordinateX} Y:{fishToTheNetButton.CoordinateY}");

            releaseFishButton = ConfigToCoordinates("ReleaseFishButton");
            LogToListbox($"Release Fish botton coord: X:{releaseFishButton.CoordinateX} Y:{releaseFishButton.CoordinateY}");

            craftedOkButton = ConfigToCoordinates("CraftedOkButton");
            LogToListbox($"Getting crafted button coord: X:{craftedOkButton.CoordinateX} Y:{craftedOkButton.CoordinateY}");

            craftButton = ConfigToCoordinates("CraftButton");
            LogToListbox($"Getting craft button coord: X:{craftButton.CoordinateX} Y:{craftButton.CoordinateY}");
        }

        private Color GetColorAt(Point location)
        {
            using (Graphics gdest = Graphics.FromImage(screenPixel))
            {
                using (Graphics gsrc = Graphics.FromHwnd(IntPtr.Zero))
                {
                    IntPtr hSrcDC = gsrc.GetHdc();
                    IntPtr hDC = gdest.GetHdc();
                    int retval = BitBlt(hDC, 0, 0, 1, 1, hSrcDC, location.X, location.Y, (int)CopyPixelOperation.SourceCopy);
                    gdest.ReleaseHdc();
                    gsrc.ReleaseHdc();
                }
            }

            return screenPixel.GetPixel(0, 0);
        }

        private void cbBrewing_CheckedChanged(object sender, EventArgs e)
        {
            if (cbBrewing.Checked)
            {
                cbAuto.Checked = false;
                cbFloat.Checked = false;
                cbSeaFishing.Checked = false;
            }
        }

        private void cbSeaFishing_CheckedChanged(object sender, EventArgs e)
        {
            if (cbSeaFishing.Checked)
            {
                cbFloat.Checked = false;
                cbBrewing.Checked = false;
            }
        }

        private void cbFloat_CheckedChanged(object sender, EventArgs e)
        {
            if (cbFloat.Checked)
            {
                cbSeaFishing.Checked = false;
                cbBrewing.Checked = false;
            }
        }
        private void cbAuto_CheckedChanged(object sender, EventArgs e)
        {
            if (cbAuto.Checked)
            {
                cbBrewing.Checked = false;
            }
        }

        private  bool IsReadyToCast()
        {
            var result = false;
            var imageBytes = CaptureScreenHelper.CaptureScreen(windowLeft + 500, windowBottom - 80, 250, 80);
            var engine = new Engine(@"tessdata", Language.Russian, EngineMode.Default);
            var resultText = string.Empty;
            using (var img = TesseractOCR.Pix.Image.LoadFromMemoryInternal(imageBytes, 0, imageBytes.Length)) {
                var textFromImage = engine.Process(img);
                resultText = textFromImage.Text;
            }
            
            result = resultText.Contains("снасть") || resultText.Contains("готова") || resultText.Contains("заброс");
            
            return result;
        }

        private bool IsFishValid()
        {
            var result = false;
            var imageBytes = CaptureScreenHelper.CaptureScreen(windowLeft, windowTop, windowWidth, 200);
            var detector = new TemplateMatchDetector();
            result = detector.DetectValid(imageBytes, "template.png");
            if (!result)
            {
                result = detector.DetectValid(imageBytes, "template_blue.png");
            }

            return result;
        }

        private bool ConfigToColour(Color col, string configName)
        {
            var result_R = true;
            var result_G = true;
            var result_B = true;
            var configColor = configuration.Colors.Where(x => x.Name == configName).FirstOrDefault();
            foreach (var item in configColor.Colors)
            {
                switch (item.Name)
                {
                    case "R":
                        result_R = item.Disabled ? true : (item.GreaterLess == "Greater" && col.R > item.Value) || (item.GreaterLess == "Less" && col.R < item.Value);
                        break;
                    case "G":
                        result_G = item.Disabled ? true : (item.GreaterLess == "Greater" && col.G > item.Value) || (item.GreaterLess == "Less" && col.G < item.Value);
                        break;
                    case "B":
                        result_B = item.Disabled ? true : (item.GreaterLess == "Greater" && col.B > item.Value) || (item.GreaterLess == "Less" && col.B < item.Value);
                        break;
                }


            }
            return result_R && result_G && result_B;
        }

        private void LogToListbox(string message)
        {
            listBox1.Items.Insert(0, DateTime.Now.ToString("dd:MM:yyyy hh:mm:ss.m") + ": " + message);
        }

        private LocationObject ConfigToCoordinates(string configName)
        {
            var result = new LocationObject();
            var cfgObject = configuration.Coordinates.Where(x => x.Name == configName).FirstOrDefault();
            if (cfgObject != null)
            {
                switch (cfgObject.CoordinateX.RelatedTo)
                {
                    case "Left":
                        result.CoordinateX = windowLeft + cfgObject.CoordinateX.Offset;
                        break;
                    case "Right":
                        result.CoordinateX = windowRight + cfgObject.CoordinateX.Offset;
                        break;
                    case "Center":
                        result.CoordinateX = windowCenterX + cfgObject.CoordinateX.Offset;
                        break;
                    default:
                        result.CoordinateX = windowLeft + cfgObject.CoordinateX.Offset;
                        break;
                }

                switch (cfgObject.CoordinateY.RelatedTo)
                {
                    case "Top":
                        result.CoordinateY = windowTop + cfgObject.CoordinateY.Offset;
                        break;
                    case "Bottom":
                        result.CoordinateY = windowBottom + cfgObject.CoordinateY.Offset;
                        break;
                    case "Center":
                        result.CoordinateY = windowCenterY + cfgObject.CoordinateY.Offset;
                        break;
                    default:
                        result.CoordinateY = windowTop + cfgObject.CoordinateY.Offset;
                        break;
                }

                result.Width = cfgObject.Width;
                result.Height = cfgObject.Height;
            }
            return result;
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
        }
    }
}