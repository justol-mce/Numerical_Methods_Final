function NumericalMethods()

clc;
clear;
close all;

%% =========================================================
% MAIN WINDOW
%% =========================================================

fig = uifigure( ...
    'Name','Numerical Methods MATLAB', ...
    'Position',[100 50 1600 900], ...
    'Color',[0.08 0.1 0.15]);

%% =========================================================
% MAIN LAYOUT
%% =========================================================

mainGrid = uigridlayout(fig,[1 1]);

%% =========================================================
% TAB GROUP
%% =========================================================

tg = uitabgroup(mainGrid);

rootTab = uitab(tg,'Title','Root Finding');
matrixTab = uitab(tg,'Title','Matrix');

%% =========================================================
% ROOT TAB GRID
%% =========================================================

rootGrid = uigridlayout(rootTab,[2 2]);

rootGrid.RowHeight = {240,'1x'};
rootGrid.ColumnWidth = {450,'1x'};

rootGrid.Padding = [10 10 10 10];
rootGrid.RowSpacing = 10;
rootGrid.ColumnSpacing = 10;

%% =========================================================
% CONTROL PANEL
%% =========================================================

controlPanel = uipanel(rootGrid,...
    'Title','Control Panel',...
    'FontWeight','bold',...
    'ForegroundColor','white',...
    'BackgroundColor',[0.12 0.14 0.2]);

controlPanel.Layout.Row = 1;
controlPanel.Layout.Column = [1 2];

controlGrid = uigridlayout(controlPanel,[4 6]);

controlGrid.RowHeight = {35 35 35 40};

controlGrid.ColumnWidth = {100,220,100,220,140,140};

controlGrid.Padding = [20 20 20 20];
controlGrid.RowSpacing = 15;
controlGrid.ColumnSpacing = 12;

%% =========================================================
% EQUATION
%% =========================================================

lbl1 = uilabel(controlGrid,'Text','Equation','FontColor','white','FontWeight','bold','FontSize',14);
lbl1.Layout.Row = 1; lbl1.Layout.Column = 1;

eqDropdown = uidropdown(controlGrid,'Items',{
    'x^3 - x - 2'
    'sin(x) - 0.5'
    'exp(x) - 3'
    'x^2 - 4'
    'cos(x) - x'
    'log(x) - 1'
    'sqrt(x) - 2'
    'Custom'},'FontSize',13);

eqDropdown.Layout.Row = 1;
eqDropdown.Layout.Column = 2;

%% =========================================================
% CUSTOM EQUATION
%% =========================================================

lbl2 = uilabel(controlGrid,'Text','Custom Eq','FontColor','white','FontWeight','bold','FontSize',14);
lbl2.Layout.Row = 1; lbl2.Layout.Column = 3;

eqField = uieditfield(controlGrid,'text','Value','x^2 - 4','Editable','off','FontSize',13);
eqField.Layout.Row = 1; eqField.Layout.Column = 4;

%% =========================================================
% METHOD
%% =========================================================

lbl3 = uilabel(controlGrid,'Text','Method','FontColor','white','FontWeight','bold','FontSize',14);
lbl3.Layout.Row = 2; lbl3.Layout.Column = 1;

methodDropdown = uidropdown(controlGrid,'Items',{
    'Incremental'
    'Bisection'
    'Regula Falsi'
    'Newton Raphson'
    'Secant'},'FontSize',13);

methodDropdown.Layout.Row = 2;
methodDropdown.Layout.Column = 2;

%% =========================================================
% XL / XU / TOL
%% =========================================================

xlField = uieditfield(controlGrid,'numeric','Value',-10);
xlField.Layout.Row = 3; xlField.Layout.Column = 2;

xuField = uieditfield(controlGrid,'numeric','Value',10);
xuField.Layout.Row = 3; xuField.Layout.Column = 4;

tolField = uieditfield(controlGrid,'numeric','Value',0.0001);
tolField.Layout.Row = 4; tolField.Layout.Column = 2;

%% =========================================================
% BUTTONS
%% =========================================================

solveBtn = uibutton(controlGrid,'push','Text','SOLVE','BackgroundColor',[0 0.6 1],'FontColor','white');
solveBtn.Layout.Row = 4; solveBtn.Layout.Column = 3;

clearBtn = uibutton(controlGrid,'push','Text','CLEAR','BackgroundColor',[0.9 0.2 0.2],'FontColor','white');
clearBtn.Layout.Row = 4; clearBtn.Layout.Column = 4;

%% =========================================================
% TABLE
%% =========================================================

tablePanel = uipanel(rootGrid,'Title','Iterations','FontWeight','bold','ForegroundColor','white','BackgroundColor',[0.12 0.14 0.2]);
tablePanel.Layout.Row = 2; tablePanel.Layout.Column = 1;

tableGrid = uigridlayout(tablePanel,[1 1]);

columns = {'i','XL','XR','XU','f(XL)','f(XR)','Error %'};

resultTable = uitable(tableGrid,'ColumnName',columns,'FontSize',11,'RowStriping','on');

%% =========================================================
% GRAPH
%% =========================================================

graphPanel = uipanel(rootGrid,'Title','Graph Visualization','FontWeight','bold','ForegroundColor','white','BackgroundColor',[0.12 0.14 0.2]);
graphPanel.Layout.Row = 2; graphPanel.Layout.Column = 2;

graphGrid = uigridlayout(graphPanel,[1 1]);
ax = uiaxes(graphGrid);
grid(ax,'on');
ax.Color = [0.95 0.97 1];

%% =========================================================
% MATRIX TAB
%% =========================================================

matrixGrid = uigridlayout(matrixTab,[2 3]);
matrixGrid.RowHeight = {160,'1x'};
matrixGrid.ColumnWidth = {'1x','1x','1.2x'};

%% =========================================================
% MATRIX CONTROL PANEL
%% =========================================================

matrixControlPanel = uipanel(matrixGrid,'Title','Matrix Operations','FontWeight','bold','FontSize',15,'ForegroundColor','white','BackgroundColor',[0.12 0.14 0.2]);
matrixControlPanel.Layout.Row = 1;
matrixControlPanel.Layout.Column = [1 3];

%% =========================================================
% CONTROL GRID (FIXED)
%% =========================================================

matrixControlGrid = uigridlayout(matrixControlPanel,[2 6]); % ✅ FIXED (was 2x5)

matrixControlGrid.RowHeight = {30 35};
matrixControlGrid.Padding = [10 10 10 10];
matrixControlGrid.RowSpacing = 8;
matrixControlGrid.ColumnSpacing = 10;

matrixControlGrid.ColumnWidth = {140,240,170,170,140,140}; % ✅ FIXED

%% =========================================================
% OPERATION
%% =========================================================

matrixLabel = uilabel(matrixControlGrid,'Text','Operation','FontColor','white','FontWeight','bold','FontSize',14);
matrixLabel.Layout.Row = 1; matrixLabel.Layout.Column = 1;

matrixDropdown = uidropdown(matrixControlGrid,'Items',{
    'Addition','Multiplication','Transpose','Determinant','Inverse','Adjoint','Power','Equation'},'FontSize',14);

matrixDropdown.Layout.Row = 1;
matrixDropdown.Layout.Column = 2;

%% =========================================================
% POWER (FIXED POSITION)
%% =========================================================

powerLabel = uilabel(matrixControlGrid,'Text','Power','FontColor','white','FontWeight','bold','FontSize',14);
powerLabel.Layout.Row = 1; powerLabel.Layout.Column = 5;

powerDropdown = uidropdown(matrixControlGrid,'Items',{'2','3','4','5','6','7','8','9','10'},'Value','2','Enable','off','FontSize',14);
powerDropdown.Layout.Row = 1;
powerDropdown.Layout.Column = 6;

%% =========================================================
% BUTTONS
%% =========================================================

computeBtn = uibutton(matrixControlGrid,'push','Text','COMPUTE','BackgroundColor',[0 0.65 1],'FontColor','white');
computeBtn.Layout.Row = 2; computeBtn.Layout.Column = 4;

clearMatrixBtn = uibutton(matrixControlGrid,'push','Text','CLEAR','BackgroundColor',[0.9 0.2 0.2],'FontColor','white');
clearMatrixBtn.Layout.Row = 2; clearMatrixBtn.Layout.Column = 5;

%% =========================================================
% MATRIX A / B / RESULT (UNCHANGED)
%% =========================================================

matrixAPanel = uipanel(matrixGrid,'Title','Matrix A');
matrixAPanel.Layout.Row = 2; matrixAPanel.Layout.Column = 1;
matrixAGrid = uigridlayout(matrixAPanel,[1 1]);
matrixA = uitextarea(matrixAGrid,'Value',{'1 2';'3 4'});

matrixBPanel = uipanel(matrixGrid,'Title','Matrix B');
matrixBPanel.Layout.Row = 2; matrixBPanel.Layout.Column = 2;
matrixBGrid = uigridlayout(matrixBPanel,[1 1]);
matrixB = uitextarea(matrixBGrid,'Value',{'5 6';'7 8'});

matrixResultPanel = uipanel(matrixGrid,'Title','Result');
matrixResultPanel.Layout.Row = 2; matrixResultPanel.Layout.Column = 3;
matrixResultGrid = uigridlayout(matrixResultPanel,[1 1]);
matrixResult = uitextarea(matrixResultGrid,'Editable','off');

%% =========================================================
% CALLBACK FIX (UNCHANGED LOGIC)
%% =========================================================

eqDropdown.ValueChangedFcn = @(src,event) toggleCustom();
solveBtn.ButtonPushedFcn = @(btn,event) solveRoot();
clearBtn.ButtonPushedFcn = @(btn,event) clearTable();
computeBtn.ButtonPushedFcn = @(btn,event) computeMatrix();
clearMatrixBtn.ButtonPushedFcn = @(btn,event) clearMatrix();

matrixDropdown.ValueChangedFcn = @(src,event) togglePower();

togglePower();
%% =========================================================
% FUNCTIONS
%% =========================================================

function toggleCustom()

    if strcmp(eqDropdown.Value,'Custom')
        eqField.Editable = 'on';
    else
        eqField.Editable = 'off';
    end

end

%% =========================================================

function expr = getEquation()

    if strcmp(eqDropdown.Value,'Custom')
        expr = eqField.Value;
    else
        expr = eqDropdown.Value;
    end

    % REMOVE DOUBLE CONVERSION BUG FIRST
    expr = char(expr);

    % ONLY FIX IF NOT ALREADY SAFE
    expr = strrep(expr,'^','.^');
    expr = strrep(expr,'*','.*');
    expr = strrep(expr,'/','./');

end

%% =========================================================

function f = parseFunction(expr)

    f = str2func(['@(x) ' expr]);

end

%% =========================================================
% ROOT SOLVER
%% =========================================================

function solveRoot()

try

    cla(ax);
    hold(ax,'on');

    resultTable.Data = {};
    xr = NaN;

    expr = getEquation();
    f = parseFunction(expr);

    xl = xlField.Value;
    xu = xuField.Value;
    tol = tolField.Value;

    method = methodDropdown.Value;

    x = linspace(xl,xu,2000);
    y = arrayfun(f,x);

    plot(ax,x,y,'LineWidth',3,'Color',[0 0.45 0.74]);
    xline(ax,0,'k--');
    yline(ax,0,'k--');
    grid(ax,'on');

    resultData = {};

    switch method

        case 'Incremental'

            step = 0.5;
            foundRoot = false;

            for i = 1:100

                x1 = xl + (i-1)*step;
                x2 = x1 + step;

                if x2 > xu
                    break;
                end

                f1 = f(x1);
                f2 = f(x2);

                resultData(end+1,:) = {i,x1,x1,x2,f1,f2,NaN};

                plot(ax,[x1 x2],[f1 f2],'b.-')

                if f1*f2 < 0

    % refine using bisection inside the bracket
    a = x1;
    b = x2;

    for j = 1:50
        xr = (a + b)/2;

        fr = f(xr);

        if abs(fr) < tol
            break;
        end

        if f(a)*fr < 0
            b = xr;
        else
            a = xr;
        end
    end

    foundRoot = true;

    scatter(ax,xr,0,120,'red','filled');

    text(ax,xr,0,...
        [' Root ~ ' num2str(xr,'%.6f')],...
        'Color','red','FontWeight','bold');

    break;
end
            end

            if ~foundRoot
                uialert(fig,'No root found in interval','Incremental Method');
            end

        case 'Bisection'

            xr_old = xl;

            for i = 1:100

                xr = (xl + xu)/2;

                fxl = f(xl);
                fxr = f(xr);

                if i == 1
                    ea = NaN;
                else
                    ea = abs((xr - xr_old)/xr)*100;
                end

                resultData(end+1,:) = {i,xl,xr,xu,fxl,fxr,ea};

                scatter(ax,xr,0,80,'red','filled');

                if fxl * fxr < 0
                    xu = xr;
                else
                    xl = xr;
                end

                if abs(fxr) < tol || ea < tol*100
                    break;
                end

                xr_old = xr;
            end

        case 'Regula Falsi'

            xr_old = xl;
            fxl = f(xl);
            fxu = f(xu);

            for i = 1:100

                xr = xu - (fxu*(xl-xu))/(fxl-fxu);
                fxr = f(xr);

                ea = abs((xr-xr_old)/xr)*100;

                resultData(end+1,:) = {i,xl,xr,xu,fxl,fxr,ea};

                scatter(ax,xr,0,80,'g','filled');

                if fxl*fxr < 0
                    xu = xr;
                else
                    xl = xr;
                end

                if abs(fxr) < tol || ea < tol*100
                    break;
                end

                xr_old = xr;
            end

        case 'Newton Raphson'

            x0 = (xl + xu)/2;
            h = 1e-6;

            for i = 1:100

                fx = f(x0);
                dfx = (f(x0+h)-f(x0-h))/(2*h);

                if abs(dfx) < 1e-12
                    break;
                end

                x1 = x0 - fx/dfx;

                ea = abs((x1-x0)/x1)*100;

                resultData(end+1,:) = {i,x0,x1,'-',fx,f(x1),ea};

                scatter(ax,x1,0,80,'m','filled');

                if abs(f(x1)) < tol || ea < tol*100
                    xr = x1;
                    break;
                end

                x0 = x1;
            end

        case 'Secant'

            x0 = xl;
            x1 = xu;

            for i = 1:100

                fx0 = f(x0);
                fx1 = f(x1);

                x2 = x1 - fx1*(x1-x0)/(fx1-fx0);

                ea = abs((x2-x1)/x2)*100;

                resultData(end+1,:) = {i,x0,x1,x2,fx0,fx1,ea};

                scatter(ax,x2,0,80,'c','filled');

                if abs(f(x2)) < tol || ea < tol*100
                    xr = x2;
                    break;
                end

                x0 = x1;
                x1 = x2;
            end

    end

    resultTable.Data = resultData;

    if ~isnan(xr)
        scatter(ax,xr,0,200,'red','filled');
        text(ax,xr,0,[' Root = ' num2str(xr,'%.6f')], ...
            'Color','red','FontWeight','bold');
    end

catch ME
    uialert(fig,ME.message,'Error');
end

end

%% =========================================================
% MATRIX FUNCTIONS
%% =========================================================

function computeMatrix()

    try

        A = parseMatrix(matrixA.Value);

        op = matrixDropdown.Value;

        B = [];

        switch op

            case 'Addition'

                B = parseMatrix(matrixB.Value);
                result = A + B;

            case 'Multiplication'

                B = parseMatrix(matrixB.Value);
                result = A * B;

            case 'Transpose'

                result = A';

            case 'Determinant'

                result = det(A);

            case 'Inverse'

                result = inv(A);

            case 'Adjoint'

                result = det(A)*inv(A);

            case 'Power'
                p = str2double(powerDropdown.Value);

                result = A^p;

            case 'Equation'

                B = parseMatrix(matrixB.Value);

                result = A \ B;

        end

        matrixResult.Value = splitlines(evalc('disp(result)'));

    catch ME

        uialert(fig,ME.message,'Matrix Error');

    end

end

%% =========================================================    

%% =========================================================

function A = parseMatrix(cellText)

    txt = strjoin(cellText,newline);

    rows = splitlines(txt);

    A = [];

    for i = 1:length(rows)

        nums = str2num(rows{i}); %#ok<ST2NM>

        A = [A; nums];

    end

end

%% =========================================================

function clearTable()

    resultTable.Data = {};

    cla(ax);
    hold(ax,'on');

end

%% =========================================================

function clearMatrix()

    matrixA.Value = {'1 2';'3 4'};
    matrixB.Value = {'5 6';'7 8'};

    matrixResult.Value = '';

end

%% =========================================================
% TOGGLE POWER
%% =========================================================

    function togglePower()

    op = matrixDropdown.Value;

    if strcmp(op,'Power')
        powerDropdown.Enable = 'on';
    else
        powerDropdown.Enable = 'off';
    end

end

end
    
