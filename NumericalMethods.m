function NumericalMethods()
% MATLAB Numerical Methods GUI
% Implements Root Finding Methods and Matrix Operations
% Similar functionality to main1.py

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
fig.Visible = 'on';

%% =========================================================
% MAIN LAYOUT
%% =========================================================

mainGrid = uigridlayout(fig,[1 1]);
mainGrid.Padding = [10 10 10 10];

%% =========================================================
% TAB GROUP
%% =========================================================

tg = uitabgroup(mainGrid);
tg.BackgroundColor = [0.08 0.1 0.15];

rootTab = uitab(tg,'Title','Root Finding');
rootTab.BackgroundColor = [0.08 0.1 0.15];

matrixTab = uitab(tg,'Title','Matrix');
matrixTab.BackgroundColor = [0.08 0.1 0.15];

%% =========================================================
% ROOT TAB GRID
%% =========================================================

rootGrid = uigridlayout(rootTab,[2 2]);

rootGrid.RowHeight = {280,'1x'};
rootGrid.ColumnWidth = {500,'1x'};

rootGrid.Padding = [15 15 15 15];
rootGrid.RowSpacing = 15;
rootGrid.ColumnSpacing = 15;

%% =========================================================
% CONTROL PANEL
%% =========================================================

controlPanel = uipanel(rootGrid,...
    'Title','Control Panel',...
    'FontName','Arial',...
    'FontWeight','bold',...
    'FontSize',13,...
    'ForegroundColor',[0.2 0.8 1],...
    'BackgroundColor',[0.12 0.14 0.2]);

controlPanel.Layout.Row = 1;
controlPanel.Layout.Column = [1 2];

controlGrid = uigridlayout(controlPanel,[5 3]);

controlGrid.RowHeight = {30 30 30 30 40};

controlGrid.ColumnWidth = {150 'auto' 150};

controlGrid.Padding = [20 20 20 20];
controlGrid.RowSpacing = 12;
controlGrid.ColumnSpacing = 15;

%% =========================================================
% EQUATION
%% =========================================================

lbl1 = uilabel(controlGrid,'Text','Select Equation:','FontColor',[0.2 0.8 1],'FontWeight','bold','FontSize',11);
lbl1.Layout.Row = 1; 
lbl1.Layout.Column = 1;

eqDropdown = uidropdown(controlGrid,'Items',{
    'x^3 - x - 2'
    'sin(x) - 0.5'
    'exp(x) - 3'
    'x^2 - 4'
    'cos(x) - x'
    'log(x) - 1'
    'sqrt(x) - 2'
    'Custom'},...
    'Value','x^2 - 4',...
    'FontSize',11,...
    'BackgroundColor',[0.15 0.18 0.25],...
    'FontColor',[0.8 0.8 0.8]);

eqDropdown.Layout.Row = 1;
eqDropdown.Layout.Column = [2 3];

%% =========================================================
% CUSTOM EQUATION
%% =========================================================

lbl2 = uilabel(controlGrid,'Text','Custom Equation:','FontColor',[0.2 0.8 1],'FontWeight','bold','FontSize',11);
lbl2.Layout.Row = 2; 
lbl2.Layout.Column = 1;

eqField = uieditfield(controlGrid,'text',...
    'Value','x^2 - 4',...
    'Editable','off',...
    'FontSize',11,...
    'BackgroundColor',[0.15 0.18 0.25],...
    'FontColor',[0.8 0.8 0.8]);
eqField.Layout.Row = 2; 
eqField.Layout.Column = [2 3];

%% =========================================================
% METHOD
%% =========================================================

lbl3 = uilabel(controlGrid,'Text','Select Method:','FontColor',[0.2 0.8 1],'FontWeight','bold','FontSize',11);
lbl3.Layout.Row = 3; 
lbl3.Layout.Column = 1;

methodDropdown = uidropdown(controlGrid,'Items',{
    'Incremental'
    'Bisection'
    'Regula Falsi'
    'Newton Raphson'
    'Secant'},...
    'Value','Bisection',...
    'FontSize',11,...
    'BackgroundColor',[0.15 0.18 0.25],...
    'FontColor',[0.8 0.8 0.8]);

methodDropdown.Layout.Row = 3;
methodDropdown.Layout.Column = [2 3];

%% =========================================================
% XL / XU / TOL
%% =========================================================

lbl_xl = uilabel(controlGrid,'Text','XL:','FontColor',[0.2 0.8 1],'FontWeight','bold','FontSize',11);
lbl_xl.Layout.Row = 4; 
lbl_xl.Layout.Column = 1;

xlField = uieditfield(controlGrid,'numeric',...
    'Value',-10,...
    'BackgroundColor',[0.15 0.18 0.25],...
    'FontColor',[0.8 0.8 0.8]);
xlField.Layout.Row = 4; 
xlField.Layout.Column = 2;

lbl_tol = uilabel(controlGrid,'Text','Tolerance:','FontColor',[0.2 0.8 1],'FontWeight','bold','FontSize',11);
lbl_tol.Layout.Row = 4; 
lbl_tol.Layout.Column = 3;

tolField = uieditfield(controlGrid,'numeric',...
    'Value',0.0001,...
    'BackgroundColor',[0.15 0.18 0.25],...
    'FontColor',[0.8 0.8 0.8]);
tolField.Layout.Row = 4; 
tolField.Layout.Column = 3;

%% =========================================================
% BUTTONS & XU
%% =========================================================

solveBtn = uibutton(controlGrid,'push',...
    'Text','SOLVE',...
    'BackgroundColor',[0 0.7 1],...
    'FontColor','white',...
    'FontWeight','bold',...
    'FontSize',12);
solveBtn.Layout.Row = 5; 
solveBtn.Layout.Column = 1;

lbl_xu = uilabel(controlGrid,'Text','XU:','FontColor',[0.2 0.8 1],'FontWeight','bold','FontSize',11);
lbl_xu.Layout.Row = 5; 
lbl_xu.Layout.Column = 2;

xuField = uieditfield(controlGrid,'numeric',...
    'Value',10,...
    'BackgroundColor',[0.15 0.18 0.25],...
    'FontColor',[0.8 0.8 0.8]);
xuField.Layout.Row = 5; 
xuField.Layout.Column = 2;

clearBtn = uibutton(controlGrid,'push',...
    'Text','CLEAR',...
    'BackgroundColor',[1 0.3 0.3],...
    'FontColor','white',...
    'FontWeight','bold',...
    'FontSize',12);
clearBtn.Layout.Row = 5; 
clearBtn.Layout.Column = 3;

%% =========================================================
% TABLE
%% =========================================================

tablePanel = uipanel(rootGrid,...
    'Title','Iterations Table',...
    'FontName','Arial',...
    'FontWeight','bold',...
    'FontSize',12,...
    'ForegroundColor',[0.2 0.8 1],...
    'BackgroundColor',[0.12 0.14 0.2]);
tablePanel.Layout.Row = 2; 
tablePanel.Layout.Column = 1;

tableGrid = uigridlayout(tablePanel,[1 1]);
tableGrid.Padding = [5 5 5 5];

columns = {'Iter','XL','XR','XU','f(XL)','f(XR)','Error %'};

resultTable = uitable(tableGrid,...
    'ColumnName',columns,...
    'FontSize',10,...
    'RowStriping','on',...
    'BackgroundColor',[0.15 0.18 0.25; 0.12 0.15 0.22]);
resultTable.FontColor = [0.8 0.8 0.8];

%% =========================================================
% GRAPH
%% =========================================================

graphPanel = uipanel(rootGrid,...
    'Title','Graph Visualization',...
    'FontName','Arial',...
    'FontWeight','bold',...
    'FontSize',12,...
    'ForegroundColor',[0.2 0.8 1],...
    'BackgroundColor',[0.12 0.14 0.2]);
graphPanel.Layout.Row = 2; 
graphPanel.Layout.Column = 2;

graphGrid = uigridlayout(graphPanel,[1 1]);
graphGrid.Padding = [5 5 5 5];

ax = uiaxes(graphGrid);
ax.Color = [0.05 0.07 0.12];
ax.GridColor = [0.2 0.22 0.28];
ax.XColor = [0.6 0.6 0.6];
ax.YColor = [0.6 0.6 0.6];
ax.TitleFontSizeMultiplier = 1.1;
ax.Title.Color = [0.2 0.8 1];
ax.XLabel.Color = [0.6 0.6 0.6];
ax.YLabel.Color = [0.6 0.6 0.6];

grid(ax,'on');
ax.GridAlpha = 0.15;
hold(ax,'on');

%% =========================================================
% MATRIX TAB
%% =========================================================

matrixGrid = uigridlayout(matrixTab,[2 3]);
matrixGrid.RowHeight = {140,'1x'};
matrixGrid.ColumnWidth = {'1x','1x','1.2x'};
matrixGrid.Padding = [15 15 15 15];
matrixGrid.RowSpacing = 15;
matrixGrid.ColumnSpacing = 15;

%% =========================================================
% MATRIX CONTROL PANEL
%% =========================================================

matrixControlPanel = uipanel(matrixGrid,...
    'Title','Matrix Operations',...
    'FontName','Arial',...
    'FontWeight','bold',...
    'FontSize',13,...
    'ForegroundColor',[0.2 0.8 1],...
    'BackgroundColor',[0.12 0.14 0.2]);
matrixControlPanel.Layout.Row = 1;
matrixControlPanel.Layout.Column = [1 3];

%% =========================================================
% CONTROL GRID
%% =========================================================

matrixControlGrid = uigridlayout(matrixControlPanel,[2 5]);

matrixControlGrid.RowHeight = {30 35};
matrixControlGrid.Padding = [15 15 15 15];
matrixControlGrid.RowSpacing = 10;
matrixControlGrid.ColumnSpacing = 15;

matrixControlGrid.ColumnWidth = {120 'auto' 100 'auto' 100};

%% =========================================================
% OPERATION
%% =========================================================

matrixLabel = uilabel(matrixControlGrid,...
    'Text','Operation:',...
    'FontColor',[0.2 0.8 1],...
    'FontWeight','bold',...
    'FontSize',11);
matrixLabel.Layout.Row = 1; 
matrixLabel.Layout.Column = 1;

matrixDropdown = uidropdown(matrixControlGrid,'Items',{
    'Addition','Multiplication','Transpose',...
    'Determinant','Inverse','Adjoint','Power','Equation'},...
    'Value','Multiplication',...
    'FontSize',11,...
    'BackgroundColor',[0.15 0.18 0.25],...
    'FontColor',[0.8 0.8 0.8]);

matrixDropdown.Layout.Row = 1;
matrixDropdown.Layout.Column = [2 3];

%% =========================================================
% POWER
%% =========================================================

powerLabel = uilabel(matrixControlGrid,...
    'Text','Power:',...
    'FontColor',[0.2 0.8 1],...
    'FontWeight','bold',...
    'FontSize',11);
powerLabel.Layout.Row = 1; 
powerLabel.Layout.Column = 4;

powerDropdown = uidropdown(matrixControlGrid,...
    'Items',{'2','3','4','5','6','7','8','9','10'},...
    'Value','2',...
    'Enable','off',...
    'FontSize',11,...
    'BackgroundColor',[0.15 0.18 0.25],...
    'FontColor',[0.8 0.8 0.8]);
powerDropdown.Layout.Row = 1;
powerDropdown.Layout.Column = 5;

%% =========================================================
% BUTTONS
%% =========================================================

computeBtn = uibutton(matrixControlGrid,'push',...
    'Text','COMPUTE',...
    'BackgroundColor',[0 0.7 1],...
    'FontColor','white',...
    'FontWeight','bold',...
    'FontSize',11);
computeBtn.Layout.Row = 2; 
computeBtn.Layout.Column = 2;

clearMatrixBtn = uibutton(matrixControlGrid,'push',...
    'Text','CLEAR',...
    'BackgroundColor',[1 0.3 0.3],...
    'FontColor','white',...
    'FontWeight','bold',...
    'FontSize',11);
clearMatrixBtn.Layout.Row = 2; 
clearMatrixBtn.Layout.Column = 4;

%% =========================================================
% MATRIX PANELS
%% =========================================================

matrixAPanel = uipanel(matrixGrid,...
    'Title','Matrix A',...
    'FontName','Arial',...
    'FontWeight','bold',...
    'ForegroundColor',[0.2 0.8 1],...
    'BackgroundColor',[0.12 0.14 0.2]);
matrixAPanel.Layout.Row = 2; 
matrixAPanel.Layout.Column = 1;

matrixAGrid = uigridlayout(matrixAPanel,[1 1]);
matrixAGrid.Padding = [5 5 5 5];

matrixA = uitextarea(matrixAGrid,...
    'Value',{'1 2';'3 4'},...
    'FontSize',11,...
    'BackgroundColor',[0.15 0.18 0.25],...
    'FontColor',[0.8 0.8 0.8]);

matrixBPanel = uipanel(matrixGrid,...
    'Title','Matrix B',...
    'FontName','Arial',...
    'FontWeight','bold',...
    'ForegroundColor',[0.2 0.8 1],...
    'BackgroundColor',[0.12 0.14 0.2]);
matrixBPanel.Layout.Row = 2; 
matrixBPanel.Layout.Column = 2;

matrixBGrid = uigridlayout(matrixBPanel,[1 1]);
matrixBGrid.Padding = [5 5 5 5];

matrixB = uitextarea(matrixBGrid,...
    'Value',{'5 6';'7 8'},...
    'FontSize',11,...
    'BackgroundColor',[0.15 0.18 0.25],...
    'FontColor',[0.8 0.8 0.8]);

matrixResultPanel = uipanel(matrixGrid,...
    'Title','Result',...
    'FontName','Arial',...
    'FontWeight','bold',...
    'ForegroundColor',[0.2 0.8 1],...
    'BackgroundColor',[0.12 0.14 0.2]);
matrixResultPanel.Layout.Row = 2; 
matrixResultPanel.Layout.Column = 3;

matrixResultGrid = uigridlayout(matrixResultPanel,[1 1]);
matrixResultGrid.Padding = [5 5 5 5];

matrixResult = uitextarea(matrixResultGrid,...
    'Editable','off',...
    'FontSize',11,...
    'BackgroundColor',[0.15 0.18 0.25],...
    'FontColor',[0.8 0.8 0.8]);

%% =========================================================
% CALLBACKS
%% =========================================================

eqDropdown.ValueChangedFcn = @(src,event) toggleCustom();
solveBtn.ButtonPushedFcn = @(btn,event) solveRoot();
clearBtn.ButtonPushedFcn = @(btn,event) clearTable();
computeBtn.ButtonPushedFcn = @(btn,event) computeMatrix();
clearMatrixBtn.ButtonPushedFcn = @(btn,event) clearMatrix();
matrixDropdown.ValueChangedFcn = @(src,event) togglePower();

drawnow;
togglePower();

%% =========================================================
% NESTED FUNCTIONS
%% =========================================================

    function toggleCustom()
        if strcmp(eqDropdown.Value,'Custom')
            eqField.Editable = 'on';
        else
            eqField.Editable = 'off';
        end
    end

    function expr = getEquation()
        if strcmp(eqDropdown.Value,'Custom')
            expr = eqField.Value;
        else
            expr = eqDropdown.Value;
        end
        expr = char(expr);
        expr = strrep(expr,'^','.^');
        expr = strrep(expr,'*','.*');
        expr = strrep(expr,'/','./');
    end

    function f = parseFunction(expr)
        f = str2func(['@(x) ' expr]);
    end

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

            x = linspace(xl, xu, 2000);
            y = arrayfun(f, x);

            plot(ax, x, y, 'LineWidth', 2.5, 'Color', [0 0.6 1]);
            xline(ax, 0, '--', 'Color', [0.5 0.5 0.5], 'LineWidth', 0.8, 'Alpha', 0.4);
            yline(ax, 0, '--', 'Color', [0.5 0.5 0.5], 'LineWidth', 0.8, 'Alpha', 0.4);
            
            grid(ax, 'on');
            ax.GridAlpha = 0.15;
            ax.Title.String = ['f(x) = ' expr];
            ax.XLabel.String = 'x';
            ax.YLabel.String = 'f(x)';

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
                        resultData(end+1,:) = {i, x1, x1, x2, f1, f2, NaN};
                        plot(ax, [x1 x2], [f1 f2], 'b.-', 'LineWidth', 1.2, 'MarkerSize', 3);

                        if f1*f2 < 0
                            a = x1;
                            b = x2;
                            for j = 1:50
                                xr = (a + b)/2;
                                fr = f(xr);
                                if abs(fr) < tol, break; end
                                if f(a)*fr < 0
                                    b = xr;
                                else
                                    a = xr;
                                end
                            end
                            foundRoot = true;
                            scatter(ax, xr, 0, 120, 'red', 'filled');
                            text(ax, xr, max(y)*0.1, sprintf(' Root: %.6f', xr), 'Color', 'red', 'FontWeight', 'bold');
                            break;
                        end
                    end
                    if ~foundRoot
                        uialert(fig, 'No root found in interval', 'Incremental Method');
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
                        resultData(end+1,:) = {i, xl, xr, xu, fxl, fxr, ea};
                        scatter(ax, xr, 0, 70, 'red', 'filled', 'Alpha', 0.6);

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
                        resultData(end+1,:) = {i, xl, xr, xu, fxl, fxr, ea};
                        scatter(ax, xr, 0, 70, 'g', 'filled', 'Alpha', 0.6);

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
                        resultData(end+1,:) = {i, x0, x1, '-', fx, f(x1), ea};
                        scatter(ax, x1, 0, 70, 'm', 'filled', 'Alpha', 0.6);

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
                        resultData(end+1,:) = {i, x0, x1, x2, fx0, fx1, ea};
                        scatter(ax, x2, 0, 70, 'c', 'filled', 'Alpha', 0.6);

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
                scatter(ax, xr, 0, 180, 'red', 'filled');
                text(ax, xr, max(y)*0.05, sprintf(' Root = %.6f', xr), ...
                    'Color', 'red', 'FontWeight', 'bold', 'FontSize', 11, ...
                    'BackgroundColor', [0.15 0.18 0.25], 'EdgeColor', 'red', 'Margin', 3);
            end

        catch ME
            uialert(fig, ME.message, 'Error');
        end
    end

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
            uialert(fig, ME.message, 'Matrix Error');
        end
    end

    function A = parseMatrix(cellText)
        txt = strjoin(cellText, newline);
        rows = splitlines(txt);
        A = [];

        for i = 1:length(rows)
            nums = str2num(rows{i}); %#ok<ST2NM>
            A = [A; nums];
        end
    end

    function clearTable()
        resultTable.Data = {};
        cla(ax);
        hold(ax, 'on');
        ax.Title.String = '';
        ax.XLabel.String = 'x';
        ax.YLabel.String = 'f(x)';
    end

    function clearMatrix()
        matrixA.Value = {'1 2';'3 4'};
        matrixB.Value = {'5 6';'7 8'};
        matrixResult.Value = '';
    end

    function togglePower()
        op = matrixDropdown.Value;
        if strcmp(op, 'Power')
            powerDropdown.Enable = 'on';
        else
            powerDropdown.Enable = 'off';
        end
    end

end
