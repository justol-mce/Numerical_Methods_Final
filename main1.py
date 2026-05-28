import math
import re
import tkinter as tk
from tkinter import ttk, messagebox

import numpy as np
from matplotlib.backends.backend_tkagg import FigureCanvasTkAgg
from matplotlib.figure import Figure
from matplotlib import style
style.use('seaborn-v0_8-darkgrid')

LIGHT_BG = '#f8f9fa'

def normalize_expression(expr):
    expr = expr.strip()
    if not expr:
        raise ValueError("Equation is empty")

    expr = expr.replace('^', '**')
    expr = expr.replace('X', 'x')

    if expr.count('=') > 1:
        raise ValueError("Equation can contain only one equals sign")

    if '=' in expr:
        left, right = expr.split('=', 1)
        left = left.strip()
        right = right.strip()
        expr = f"({left}) - ({right})"
    expr = re.sub(r'(?<=\d)(?=[x\(])', '*', expr)
    expr = re.sub(r'(?<=\d)(?=(sin|cos|tan|exp|log|sqrt|abs|pi|e)\b)', '*', expr)
    expr = re.sub(r'(?<=[x\)])(?=\()', '*', expr)
    expr = re.sub(r'(?<=\))(?=x)', '*', expr)
    expr = re.sub(r'(?<=x)(?=\d)', '*', expr)
    expr = re.sub(r'(?<=pi)(?=x|\()', '*', expr)

    return expr

def parse_function(expr):
    expr = normalize_expression(expr)
    def f(x):
        try:
            return eval(expr, {"__builtins__": {}}, {
                "x": x,
                "np": np,
                "math": math,
                "sin": np.sin,
                "cos": np.cos,
                "tan": np.tan,
                "exp": np.exp,
                "log": np.log,
                "log10": np.log10,
                "sqrt": np.sqrt,
                "pi": np.pi,
                "e": np.e,
                "abs": abs
            })
        except:
            return float('nan')
    return f

def refine_root(f, a, b, tol=1e-8, max_iter=60):
    fa = f(a)
    fb = f(b)
    if np.isnan(fa) or np.isnan(fb) or fa * fb > 0:
        return None

    for _ in range(max_iter):
        c = (a + b) / 2
        fc = f(c)
        if np.isnan(fc):
            return None
        if abs(fc) < tol or abs(b - a) < tol:
            return c
        if fa * fc <= 0:
            b = c
            fb = fc
        else:
            a = c
            fa = fc
    return (a + b) / 2

def find_bracket(f, xl, xu, samples=800):
    """Find a sign-changing interval inside [xl, xu], useful for linear/binomial equations."""
    xs = np.linspace(xl, xu, samples + 1)
    prev_x = None
    prev_y = None

    for x in xs:
        y = f(x)
        if not np.isfinite(y):
            continue
        if abs(y) < 1e-10:
            span = max((xu - xl) / samples, 1e-4)
            return x - span, x + span
        if prev_y is not None and prev_y * y < 0:
            return prev_x, x
        prev_x = x
        prev_y = y

    return None

def find_bracket_auto(f, xl, xu):
    bracket = find_bracket(f, xl, xu)
    if bracket is not None:
        return bracket

    # Fallback for equations whose roots are outside the typed interval.
    for low, high in [(-10, 10), (-100, 100), (-1000, 1000)]:
        bracket = find_bracket(f, low, high, samples=4000)
        if bracket is not None:
            return bracket

    return None

def has_sign_change(f, a, b):
    fa = f(a)
    fb = f(b)
    return np.isfinite(fa) and np.isfinite(fb) and fa * fb <= 0

def find_iteration_bracket(f, xl, xu, roots=None):
    if has_sign_change(f, xl, xu):
        return xl, xu

    roots = roots or find_all_roots_auto(f, xl, xu)
    for root in roots:
        for width in (0.5, 1, 2, 5, 10, 20, 50, 100):
            a = root - width
            b = root + width
            if abs(b - a) > 0.1 and has_sign_change(f, a, b):
                return a, b

    return find_bracket_auto(f, xl, xu)

def numerical_derivative(f, x, h=1e-6):
    fx1 = f(x + h)
    fx0 = f(x - h)
    if not np.isfinite(fx1) or not np.isfinite(fx0):
        return float('nan')
    return (fx1 - fx0) / (2 * h)

def add_unique_root(roots, root, tol=1e-6):
    if root is None or not np.isfinite(root):
        return
    if all(abs(root - existing) > tol for existing in roots):
        roots.append(float(root))

def find_all_roots(f, xl, xu, samples=2000, tol=1e-7):
    roots = []
    xs = np.linspace(xl, xu, samples + 1)
    ys = []

    for x in xs:
        y = f(x)
        ys.append(y if np.isfinite(y) else np.nan)

    for i in range(len(xs) - 1):
        y1 = ys[i]
        y2 = ys[i + 1]
        if np.isnan(y1) or np.isnan(y2):
            continue

        if abs(y1) < tol:
            add_unique_root(roots, xs[i], tol=1e-5)
        if y1 * y2 < 0:
            add_unique_root(roots, refine_root(f, xs[i], xs[i + 1], tol=tol), tol=1e-5)

    # Newton sweeps catch tangent/double roots such as (x - 2)^2 = 0.
    for seed in np.linspace(xl, xu, 80):
        x = seed
        for _ in range(40):
            y = f(x)
            dy = numerical_derivative(f, x)
            if not np.isfinite(y) or not np.isfinite(dy) or abs(dy) < 1e-12:
                break
            x_next = x - y / dy
            if x_next < xl - 1 or x_next > xu + 1:
                break
            if abs(x_next - x) < tol:
                x = x_next
                break
            x = x_next
        if xl - 1e-6 <= x <= xu + 1e-6 and abs(f(x)) < 1e-5:
            add_unique_root(roots, x, tol=1e-5)

    roots.sort()
    return roots

def find_all_roots_auto(f, xl, xu):
    roots = find_all_roots(f, xl, xu)
    if roots:
        return roots

    for low, high in [(-10, 10), (-100, 100), (-1000, 1000)]:
        roots = find_all_roots(f, low, high, samples=5000)
        if roots:
            return roots

    return []

def parse_matrix(str_data):
    rows = [[float(v) for v in r.split()] for r in str_data.strip().splitlines() if r.strip()]
    if not rows:
        raise ValueError("Empty matrix")
    if len({len(row) for row in rows}) != 1:
        raise ValueError("Inconsistent matrix size")
    return rows

def matrix_to_string(A):
    return "\n".join("".join(f"{float(val):10.4f}" for val in row) for row in A)

def matrix_size(A):
    return f"{len(A)} x {len(A[0])}"

def add(A, B):
    return (np.array(A) + np.array(B)).tolist()

def multiply(A, B):
    return (np.array(A) @ np.array(B)).tolist()

def matrix_power(A, exponent):
    if exponent < 1 or int(exponent) != exponent:
        raise ValueError("Exponent must be a positive integer")
    return np.linalg.matrix_power(np.array(A), int(exponent)).tolist()

def transpose(A):
    return np.array(A).T.tolist()

def minor(A, row, col):
    n = len(A)
    return [
        [A[i][j] for j in range(n) if j != col]
        for i in range(n) if i != row
    ]

def determinant(A):
    return float(np.linalg.det(np.array(A)))

def adjoint(A):
    n = len(A)
    if n == 1:
        return [[1.0]]
    return [[((-1) ** (i + j)) * determinant(minor(A, i, j)) for i in range(n)] for j in range(n)]

def inverse(A):
    return np.linalg.inv(np.array(A)).tolist()

def solve_equation(A, B):
    return np.linalg.solve(np.array(A), np.array(B)).tolist()

MATRIX_OPERATIONS = [
    "Addition",
    "Multiplication",
    "Transpose",
    "Determinant",
    "Inverse",
    "Adjoint",
    "Power",
    "Equation"
]

ROOT_EQUATIONS = [
    "x^3 - x - 2",
    "sin(x) - 0.5",
    "exp(x) - 3",
    "x^2 - 4",
    "cos(x) - x",
    "log(x) - 1",
    "x^2 + 5x + 6 = 0",
    "3(x+2) - 8x = x + 9",
    "Custom"
]
class NumericalMethodsApp:
    def __init__(self, root):
        self.root = root
        self.root.title("Numerical Methods")
        self.root.geometry("1600x900")
        self.root.minsize(1200, 700)
        self.iteration_points = []
        self.detected_roots = []
        self.root.configure(bg=LIGHT_BG)
        
        self.setup_styles()
        self.create_header()
        self.create_main_content()

    def setup_styles(self):
        self.ttk_style = ttk.Style()
        if "vista" in self.ttk_style.theme_names():
            self.ttk_style.theme_use("vista")

        self.ttk_style.configure('Header.TLabel', font=('Segoe UI', 20, 'bold'))
        self.ttk_style.configure('Subheader.TLabel', font=('Segoe UI', 12, 'bold'))
        self.ttk_style.configure('TLabel', font=('Segoe UI', 10))
        self.ttk_style.configure('TButton', font=('Segoe UI', 10, 'bold'), padding=8)
        self.ttk_style.configure('TEntry', padding=5)
        self.ttk_style.configure('TCombobox', padding=5)
        self.ttk_style.configure('Treeview', font=('Courier New', 9), rowheight=22)
        self.ttk_style.configure('Treeview.Heading', font=('Segoe UI', 9, 'bold'))

    def create_header(self):
        header_frame = ttk.Frame(self.root)
        header_frame.pack(fill="x", padx=0, pady=0)
        
    # Title
        title_label = ttk.Label(
            header_frame,
            text="NUMERICAL METHODS",
            style='Header.TLabel'
        )
        title_label.pack(side="left", padx=20, pady=15)
        ttk.Frame(header_frame).pack(side="left", fill="x", expand=True)
        ttk.Separator(self.root, orient='horizontal').pack(fill="x")


    def create_main_content(self):
        self.notebook = ttk.Notebook(self.root)
        self.notebook.pack(fill="both", expand=True, padx=10, pady=10)
        self.root_tab = ttk.Frame(self.notebook)
        self.matrix_tab = ttk.Frame(self.notebook)
        
        self.notebook.add(self.root_tab, text="Root Finding Methods")
        self.notebook.add(self.matrix_tab, text="Matrix Operations")
        
        self.setup_root_tab()
        self.setup_matrix_tab()
        

    def setup_root_tab(self):
        control_frame = ttk.LabelFrame(self.root_tab, text="Configuration", padding=15)
        control_frame.pack(fill="x", padx=10, pady=10)
    
        row1 = ttk.Frame(control_frame)
        row1.pack(fill="x", pady=5)
        
        ttk.Label(row1, text="Equation:", style='Subheader.TLabel').pack(side="left", padx=5)

        
        self.equation_combo = ttk.Combobox(
            row1,
            values=ROOT_EQUATIONS,
            state="readonly",
            width=22
        )
        self.equation_combo.current(0)
        self.equation_combo.pack(side="left", padx=5)
        self.equation_combo.bind("<<ComboboxSelected>>", self.update_custom_equation_field)

        ttk.Label(row1, text="Custom Eq:", style='Subheader.TLabel').pack(side="left", padx=5)
        self.eq_entry = ttk.Entry(row1, width=28)
        self.eq_entry.insert(0, "x^2 - 4")
        self.eq_entry.config(state="disabled")
        self.eq_entry.pack(side="left", padx=5)
        
        ttk.Label(row1, text="Method:", style='Subheader.TLabel').pack(side="left", padx=5)
        self.method_combo = ttk.Combobox(
            row1,
            values=["Incremental", "Bisection", "Regula Falsi", "Newton Raphson", "Secant"],
            state="readonly",
            width=18
        )
        self.method_combo.current(1)
        self.method_combo.pack(side="left", padx=5)

        row2 = ttk.Frame(control_frame)
        row2.pack(fill="x", pady=5)
        
        ttk.Label(row2, text="Lower Bound (XL):", style='Subheader.TLabel').pack(side="left", padx=5)
        self.xl_entry = ttk.Entry(row2, width=12)
        self.xl_entry.insert(0, "-10")
        self.xl_entry.pack(side="left", padx=5)
        
        ttk.Label(row2, text="Upper Bound (XU):", style='Subheader.TLabel').pack(side="left", padx=5)
        self.xu_entry = ttk.Entry(row2, width=12)
        self.xu_entry.insert(0, "10")
        self.xu_entry.pack(side="left", padx=5)
        
        ttk.Label(row2, text="Tolerance:", style='Subheader.TLabel').pack(side="left", padx=5)
        self.tol_entry = ttk.Entry(row2, width=12)
        self.tol_entry.insert(0, "0.0001")
        self.tol_entry.pack(side="left", padx=5)
        row3 = ttk.Frame(control_frame)
        row3.pack(fill="x", pady=10)
        
        solve_btn = ttk.Button(row3, text="Solve", command=self.solve_root, width=15)
        solve_btn.pack(side="left", padx=5)
        
        clear_btn = ttk.Button(row3, text="Clear", command=self.clear_root_table, width=15)
        clear_btn.pack(side="left", padx=5)
        content_frame = ttk.Frame(self.root_tab)
        content_frame.pack(fill="both", expand=True, padx=10, pady=10)
        left_frame = ttk.LabelFrame(content_frame, text="Iteration Results", padding=5)
        left_frame.pack(side="left", fill="both", expand=True, padx=5)
        tree_frame = ttk.Frame(left_frame)
        tree_frame.pack(fill="both", expand=True)
        
        columns = ("i", "XL", "XR", "XU", "f(XL)", "f(XR)", "Error %", "Product", "Status")
        self.tree = ttk.Treeview(tree_frame, columns=columns, show="headings", height=20)
        
        column_widths = {"i": 40, "XL": 90, "XR": 90, "XU": 90, "f(XL)": 80, 
                        "f(XR)": 80, "Error %": 80, "Product": 80, "Status": 80}
        
        for col in columns:
            self.tree.heading(col, text=col)
            self.tree.column(col, width=column_widths.get(col, 80), anchor="center")
       
        vsb = ttk.Scrollbar(tree_frame, orient="vertical", command=self.tree.yview)
        hsb = ttk.Scrollbar(tree_frame, orient="horizontal", command=self.tree.xview)
        self.tree.configure(yscroll=vsb.set, xscroll=hsb.set)
        
        self.tree.grid(row=0, column=0, sticky="nsew")
        vsb.grid(row=0, column=1, sticky="ns")
        hsb.grid(row=1, column=0, sticky="ew")
        
        tree_frame.grid_rowconfigure(0, weight=1)
        tree_frame.grid_columnconfigure(0, weight=1)
        
        right_frame = ttk.LabelFrame(content_frame, text="Function Visualization", padding=5)
        right_frame.pack(side="right", fill="both", expand=True, padx=5)
        
        self.fig = Figure(figsize=(6, 6), dpi=100, facecolor='#f8f9fa')
        self.ax = self.fig.add_subplot(111)
        self.ax.set_title("f(x) Graph", fontsize=12, fontweight='bold')
        self.ax.grid(True, alpha=0.3)
        
        self.canvas = FigureCanvasTkAgg(self.fig, master=right_frame)
        self.canvas.get_tk_widget().pack(fill="both", expand=True)
        self.canvas.draw()

    def update_custom_equation_field(self, event=None):
        if self.equation_combo.get() == "Custom":
            self.eq_entry.config(state="normal")
            self.eq_entry.focus_set()
        else:
            self.eq_entry.config(state="disabled")

    def get_selected_equation(self):
        equation = self.equation_combo.get()
        if equation == "Custom":
            return self.eq_entry.get().strip()
        return equation

    def solve_root(self):
        try:
            self.iteration_points.clear()
            self.detected_roots = []
            for row in self.tree.get_children():
                self.tree.delete(row)
            
            expr = self.get_selected_equation()
            if not expr:
                messagebox.showwarning("Input Error", "Please enter an equation")
                return
            
            f = parse_function(expr)
            
            xl = float(self.xl_entry.get())
            xu = float(self.xu_entry.get())
            tol = float(self.tol_entry.get())
            
            if xl >= xu:
                messagebox.showerror("Input Error", "XL must be less than XU")
                return
            if tol <= 0:
                messagebox.showerror("Input Error", "Tolerance must be greater than zero")
                return
            
            method = self.method_combo.get()
            self.detected_roots = find_all_roots_auto(f, xl, xu)

            if method in ("Bisection", "Regula Falsi"):
                bracket = find_iteration_bracket(f, xl, xu, self.detected_roots)
                if bracket is None:
                    self.plot_graph(f, xl, xu)
                    self.insert_detected_roots()
                    messagebox.showerror(
                        "Input Error",
                        "This method needs a sign-changing bracket. Detected roots are listed in the table if found."
                    )
                    return
                xl, xu = bracket
            
            if method == "Bisection":
                self.bisection(f, xl, xu, tol)
            elif method == "Regula Falsi":
                self.regula_falsi(f, xl, xu, tol)
            elif method == "Incremental":
                self.incremental(f, xl, xu)
            elif method in ("Newton-Raphson", "Newton Raphson"):
                self.newton(f, xl, tol)
            elif method == "Secant":
                self.secant(f, xl, xu, tol)
            
            self.plot_graph(f, xl, xu)
            self.insert_detected_roots()
            
        except ValueError as e:
            messagebox.showerror("Input Error", str(e) or "Please enter valid numerical values")
        except Exception as e:
            messagebox.showerror("Error", f"An error occurred: {str(e)}")

    def insert_detected_roots(self):
        if not self.detected_roots:
            return
        for root in self.detected_roots:
            self.tree.insert("", "end", values=(
                "Root", "-", f"{root:.10f}", "-", "-", "0.000000",
                "-", "-", "Detected real root"
            ))

    def incremental(self, f, xl, xu):
        step = max((xu - xl) / 200.0, 0.01)
        i = 1
        x = xl
        while x < xu:
            fx = f(x)
            fx2 = f(x + step)
            remark = "Possible Root Detected ✓" if fx * fx2 < 0 else "Go to Next Interval →"
            self.tree.insert("", "end", values=(
                i, f"{x:.6f}", f"{x+step:.6f}", "-", 
                f"{fx:.6f}", f"{fx2:.6f}", "-", 
                f"{fx*fx2:.6f}", remark
            ))
            x += step
            i += 1

    def bisection(self, f, xl, xu, tol):
        xr_old = xl
        for i in range(1, 100):
            xr = (xl + xu) / 2
            fxl = f(xl)
            fxr = f(xr)
            product = fxl * fxr
            ea = abs((xr - xr_old) / xr) * 100 if i > 1 else float('inf')
            remark = "Root in LEFT interval → Move XU = XR" if product < 0 else "Root in RIGHT interval → Move XL = XR"
            
            self.tree.insert("", "end", values=(
                i, f"{xl:.6f}", f"{xr:.6f}", f"{xu:.6f}",
                f"{fxl:.6f}", f"{fxr:.6f}",
                f"{ea:.6f}" if ea != float('inf') else "---",
                f"{product:.6f}", remark
            ))

            self.iteration_points.append((xr, fxr))
            self.plot_graph(f, xl, xu, xr)
            self.root.update_idletasks()
            
            if product < 0:
                xu = xr
            else:
                xl = xr
            
            if abs(fxr) < 1e-10 or ea < tol:
                refined = refine_root(f, xl, xu, tol=1e-10)
                if refined is not None:
                    xr = refined
                messagebox.showinfo("Success", f"Root found at x = {xr:.10f}\nError: {ea:.6f}%")
                break
            xr_old = xr

    def regula_falsi(self, f, xl, xu, tol):
        xr_old = xl
        for i in range(1, 100):
            fxl = f(xl)
            fxu = f(xu)
            
            if abs(fxl - fxu) < 1e-10:
                messagebox.showerror("Error", "Cannot divide by zero in Regula Falsi")
                break
            
            xr = xu - (fxu * (xl - xu)) / (fxl - fxu)
            fxr = f(xr)
            product = fxl * fxr
            ea = abs((xr - xr_old) / xr) * 100 if i > 1 else float('inf')
            remark = "Root in LEFT interval → Move XU = XR" if product < 0 else "Root in RIGHT interval → Move XL = XR" 
            
            self.tree.insert("", "end", values=(
                i, f"{xl:.6f}", f"{xr:.6f}", f"{xu:.6f}",
                f"{fxl:.6f}", f"{fxr:.6f}",
                f"{ea:.6f}" if ea != float('inf') else "---",
                f"{product:.6f}", remark
            ))
            
            self.iteration_points.append((xr, fxr))
            self.plot_graph(f, xl, xu, xr)
            self.root.update_idletasks()

            if product < 0:
                xu = xr
            else:
                xl = xr
            
            if abs(fxr) < 1e-10 or ea < tol:
                refined = refine_root(f, xl, xu, tol=1e-10)
                if refined is not None:
                    xr = refined
                messagebox.showinfo("Success", f"Root found at x = {xr:.10f}\nError: {ea:.6f}%")
                break
            xr_old = xr

    def newton(self, f, x0, tol):
        h = 1e-6
        for i in range(1, 100):
            df = (f(x0 + h) - f(x0)) / h
            
            if abs(df) < 1e-10:
                messagebox.showerror("Error", "Derivative too small - cannot converge")
                break
            
            x1 = x0 - f(x0) / df

            self.iteration_points.append((x1, f(x1)))
            self.plot_graph(f, x0, x1, x1)
            self.root.update_idletasks()

            ea = abs((x1 - x0) / x1) * 100 if x1 != 0 else 0
            
            self.tree.insert("", "end", values=(
                i, f"{x0:.6f}", f"{x1:.6f}", "-",
                f"{f(x0):.6f}", f"{f(x1):.6f}",
                f"{ea:.6f}", "-", "Go to Next Iteration →"
            ))
            
            if abs(f(x1)) < 1e-10 or ea < tol:
                messagebox.showinfo("Success", f"Root found at x = {x1:.10f}\nError: {ea:.6f}%")
                break
            x0 = x1
    
    def secant(self, f, x0, x1, tol):
        for i in range(1, 100):
            fx0 = f(x0)
            fx1 = f(x1)
            
            if abs(fx1 - fx0) < 1e-10:
                messagebox.showerror("Error", "Function values too close - cannot converge")
                break
            
            x2 = x1 - (fx1 * (x1 - x0)) / (fx1 - fx0)
            ea = abs((x2 - x1) / x2) * 100 if x2 != 0 else 0
            
            self.tree.insert("", "end", values=(
                i, f"{x0:.6f}", f"{x1:.6f}", f"{x2:.6f}",
                f"{fx0:.6f}", f"{fx1:.6f}",
                f"{ea:.6f}", "-", "Go to Next Iteration →"
            ))
            
            if abs(f(x2)) < 1e-10 or ea < tol:
                messagebox.showinfo("Success", f"Root found at x = {x2:.10f}\nError: {ea:.6f}%")
                break
            x0, x1 = x1, x2

    def clear_root_table(self):
        for row in self.tree.get_children():
            self.tree.delete(row)

    def plot_graph(self, f, xl, xu, xr=None):
        self.ax.clear()
        base_margin = max((xu - xl) * 0.8, 1)

        if xr is not None:
            zoom_factor = max(base_margin * 0.3, 0.2)
            x_center = xr
        else:
            zoom_factor = base_margin
            x_center = (xl + xu) / 2

        x_min = x_center - zoom_factor
        x_max = x_center + zoom_factor

        x = np.linspace(x_min, x_max, 800)

        y = np.array([f(val) if np.isfinite(f(val)) else np.nan for val in x])

        self.ax.plot(x, y, linewidth=2.5, label="f(x)")

        if xr is not None:
            self.ax.scatter([xr], [0], color="green", s=120, zorder=5, label="Current XR")
            self.ax.scatter([xr], [0], color="green", s=250, alpha=0.25, zorder=4)
            self.ax.axvline(xr, linestyle=":", color="green", alpha=0.7)
        self.ax.axhline(0, color="black", linewidth=1)
        self.ax.axvline(0, color="black", linewidth=1)
        self.ax.axvline(xl, linestyle="--", color="black", alpha=0.7, label="XL")
        self.ax.axvline(xu, linestyle="--", color="red", alpha=0.7, label="XU")

        roots = []
        for i in range(len(x) - 1):
            if np.isnan(y[i]) or np.isnan(y[i + 1]):
                continue
            if y[i] * y[i + 1] < 0:
                refined = refine_root(f, x[i], x[i + 1], tol=1e-10)
                if refined is not None:
                    close = False
                    for existing in roots:
                        if abs(existing - refined) < 1e-6:
                            close = True
                            break
                    if not close:
                        roots.append(refined)

        for root in self.detected_roots:
            if x_min <= root <= x_max:
                close = False
                for existing in roots:
                    if abs(existing - root) < 1e-6:
                        close = True
                        break
                if not close:
                    roots.append(root)

        if roots:
            self.ax.scatter(roots, [0] * len(roots), color="green", s=80, label="Detected Roots")
            self.ax.scatter(roots, [0] * len(roots), color="black", s=30, zorder=5)

        valid_y = y[np.isfinite(y)]
        if len(valid_y) > 0:
            y_center = np.mean(valid_y)
            y_range = np.max(valid_y) - np.min(valid_y)
            if y_range > 50:
                self.ax.set_ylim(y_center - 25, y_center + 25)
            else:
                self.ax.set_ylim(np.min(valid_y) - 1, np.max(valid_y) + 1)

        self.ax.set_xlim(x_min, x_max)
        self.ax.set_title("Root Visualization")
        self.ax.set_xlabel("x")
        self.ax.set_ylabel("f(x)")
        self.ax.grid(True, alpha=0.4)
        self.ax.minorticks_on()
        self.ax.legend()
        self.fig.tight_layout()
        self.canvas.draw()

    def setup_matrix_tab(self):
        control_frame = ttk.LabelFrame(self.matrix_tab, text="Matrix Operations", padding=15)
        control_frame.pack(fill="x", padx=10, pady=10)
        row1 = ttk.Frame(control_frame)
        row1.pack(fill="x", pady=5)
        
        ttk.Label(row1, text="Operation:", style='Subheader.TLabel').pack(side="left", padx=5)
        self.matrix_combo = ttk.Combobox(row1, values=MATRIX_OPERATIONS, state="readonly", width=25)
        self.matrix_combo.current(0)
        self.matrix_combo.pack(side="left", padx=5)
        self.matrix_power_label = ttk.Label(row1, text="Power:", style='Subheader.TLabel')
        self.matrix_power_combo = ttk.Combobox(row1, values=["2", "3", "4"], state="readonly", width=5)
        self.matrix_power_combo.current(0)
        
        compute_btn = ttk.Button(row1, text="Compute", command=self.compute_matrix, width=15)
        compute_btn.pack(side="left", padx=5)
        
        clear_btn = ttk.Button(row1, text="Clear", command=self.clear_matrix, width=15)
        clear_btn.pack(side="left", padx=5)
        
        size_frame = ttk.Frame(control_frame)
        size_frame.pack(fill="x", pady=5)
        self.matrix_combo.bind("<<ComboboxSelected>>", lambda e: self.on_matrix_op_change())
        self.on_matrix_op_change()

        ttk.Label(size_frame, text="Target:", style='Subheader.TLabel').pack(side="left", padx=5)
        self.matrix_target_combo = ttk.Combobox(size_frame, values=["A", "B"], state="readonly", width=5)
        self.matrix_target_combo.current(0)
        self.matrix_target_combo.pack(side="left", padx=5)

        ttk.Label(size_frame, text="Rows:", style='Subheader.TLabel').pack(side="left", padx=5)
        self.matrix_rows_entry = ttk.Entry(size_frame, width=4)
        self.matrix_rows_entry.insert(0, "2")
        self.matrix_rows_entry.pack(side="left", padx=2)

        ttk.Label(size_frame, text="Cols:", style='Subheader.TLabel').pack(side="left", padx=5)
        self.matrix_cols_entry = ttk.Entry(size_frame, width=4)
        self.matrix_cols_entry.insert(0, "2")
        self.matrix_cols_entry.pack(side="left", padx=2)

        resize_btn = ttk.Button(size_frame, text="Set Size", command=self.resize_matrix, width=12)
        resize_btn.pack(side="left", padx=10)
        input_frame = ttk.LabelFrame(self.matrix_tab, text="Matrix Input", padding=10)
        input_frame.pack(fill="both", expand=True, padx=10, pady=5)
        left_input = ttk.Frame(input_frame)
        left_input.pack(side="left", fill="both", expand=True, padx=5)
        
        ttk.Label(left_input, text="Matrix A :", 
                 style='Subheader.TLabel').pack(anchor="w", pady=5)
        self.matrix_a = tk.Text(left_input, height=10, width=40, wrap=tk.WORD,
                               font=('Courier New', 10))
        self.matrix_a.pack(fill="both", expand=True)
        self.matrix_a.insert("1.0", "1 2\n3 4")
        self.matrix_a.bind("<KeyRelease>", self.update_matrix_size_labels)
        self.matrix_a_size_label = ttk.Label(left_input, text="Matrix A Size: 2 x 2", style='Subheader.TLabel')
        self.matrix_a_size_label.pack(anchor="w", pady=5)
        right_input = ttk.Frame(input_frame)
        right_input.pack(side="right", fill="both", expand=True, padx=5)
        
        ttk.Label(right_input, text="Matrix B (if needed):", 
                 style='Subheader.TLabel').pack(anchor="w", pady=5)
        self.matrix_b = tk.Text(right_input, height=10, width=40, wrap=tk.WORD,
                               font=('Courier New', 10))
        self.matrix_b.pack(fill="both", expand=True)
        self.matrix_b.insert("1.0", "5 6\n7 8")
        self.matrix_b.bind("<KeyRelease>", self.update_matrix_size_labels)
        self.matrix_b_size_label = ttk.Label(right_input, text="Matrix B Size: 2 x 2", style='Subheader.TLabel')
        self.matrix_b_size_label.pack(anchor="w", pady=5)
        result_frame = ttk.LabelFrame(self.matrix_tab, text="Result", padding=10)
        result_frame.pack(fill="both", expand=True, padx=10, pady=5)
        
        self.matrix_result = tk.Text(result_frame, height=12, width=100,
                                    font=('Courier New', 10), wrap=tk.WORD)
        scrollbar = ttk.Scrollbar(result_frame, command=self.matrix_result.yview)
        scrollbar.pack(side="right", fill="y")
        self.matrix_result.pack(side="left", fill="both", expand=True)
        self.matrix_result.config(yscrollcommand=scrollbar.set)
        self.update_matrix_size_labels()

    def matrix_size_from_text(self, text):
        if not text.strip():
            return "Empty"
        try:
            return matrix_size(parse_matrix(text))
        except Exception:
            return "Invalid"

    def update_matrix_size_labels(self, event=None):
        self.matrix_a_size_label.config(
            text=f"Matrix A Size: {self.matrix_size_from_text(self.matrix_a.get('1.0', 'end'))}"
        )
        self.matrix_b_size_label.config(
            text=f"Matrix B Size: {self.matrix_size_from_text(self.matrix_b.get('1.0', 'end'))}"
        )

    def on_matrix_op_change(self):
        """Show exponent selector when Power is selected; otherwise hide it.
        Also force target to A and disable target selection when powering."""
        op = self.matrix_combo.get()
        if op == "Power":
            try:
                self.matrix_power_label.pack(side="left", padx=6)
                self.matrix_power_combo.pack(side="left", padx=2)
            except Exception:
                pass
            if hasattr(self, 'matrix_target_combo'):
                try:
                    self.matrix_target_combo.set("A")
                    self.matrix_target_combo.config(state='disabled')
                except Exception:
                    pass
        else:
            try:
                self.matrix_power_label.pack_forget()
                self.matrix_power_combo.pack_forget()
            except Exception:
                pass
            if hasattr(self, 'matrix_target_combo'):
                try:
                    self.matrix_target_combo.config(state='readonly')
                except Exception:
                    pass

    def fill_matrix_widget(self, widget, rows, cols):
        text = widget.get("1.0", "end").strip()
        try:
            old_mat = parse_matrix(text)
        except Exception:
            old_mat = [[0.0 for _ in range(cols)] for _ in range(rows)]

        new_mat = [[0.0 for _ in range(cols)] for _ in range(rows)]
        for i in range(min(rows, len(old_mat))):
            for j in range(min(cols, len(old_mat[0]))):
                new_mat[i][j] = old_mat[i][j]

        widget.delete("1.0", "end")
        widget.insert("1.0", matrix_to_string(new_mat))

    def resize_matrix(self):
        try:
            rows = int(self.matrix_rows_entry.get())
            cols = int(self.matrix_cols_entry.get())
            if rows <= 0 or cols <= 0:
                raise ValueError
        except ValueError:
            messagebox.showerror("Input Error", "Rows and columns must be positive integers")
            return

        target = self.matrix_target_combo.get()
        if target == "A":
            self.fill_matrix_widget(self.matrix_a, rows, cols)
        else:
            self.fill_matrix_widget(self.matrix_b, rows, cols)

        self.update_matrix_size_labels()

    def compute_matrix(self):

        try:
            A_text = self.matrix_a.get("1.0", "end").strip()
            B_text = self.matrix_b.get("1.0", "end").strip()
            op = self.matrix_combo.get()

            if not A_text:
                messagebox.showwarning("Input Error", "Enter Matrix A")
                return

            A = parse_matrix(A_text)

            self.matrix_result.delete("1.0", "end")

            result = None

            if op == "Addition":
                if not B_text:
                    messagebox.showwarning("Input Error", "Enter Matrix B for addition")
                    return
                B = parse_matrix(B_text)

                if len(A) != len(B) or len(A[0]) != len(B[0]):
                    raise ValueError("Matrices must have same dimensions")

                result = add(A, B)

            elif op == "Multiplication":
                if not B_text:
                    messagebox.showwarning("Input Error", "Enter Matrix B for multiplication")
                    return
                B = parse_matrix(B_text)

                if len(A[0]) != len(B):
                    raise ValueError("Invalid dimensions for multiplication")

                result = multiply(A, B)

            elif op == "Transpose":
                result = transpose(A)

            elif op == "Determinant":
                if len(A) != len(A[0]):
                    raise ValueError("Square matrix required")
                det = determinant(A)
                self.matrix_result.insert("end", f"Determinant = {det:.4f}")
                messagebox.showinfo("Success", "Operation completed!")
                return

            elif op == "Inverse":
                if len(A) != len(A[0]):
                    raise ValueError("Square matrix required")
                result = inverse(A)

            elif op == "Adjoint":
                if len(A) != len(A[0]):
                    raise ValueError("Square matrix required")
                result = adjoint(A)

            elif op == "Power":
                if len(A) != len(A[0]):
                    raise ValueError("Square matrix required")
                pow_widget = getattr(self, 'matrix_power_combo', None)
                if pow_widget is None:
                    exponent = 2
                else:
                    try:
                        exponent = int(pow_widget.get() or "2")
                    except Exception:
                        exponent = 2

                result = matrix_power(A, exponent)

            elif op == "Equation":
                if not B_text:
                    messagebox.showwarning("Input Error", "Enter Matrix B for equation solver")
                    return
                B = parse_matrix(B_text)

                if len(A) != len(A[0]) or len(A[0]) != len(B):
                    raise ValueError("A must be square and B must have compatible rows")

                result = solve_equation(A, B)

            # DISPLAY
            if isinstance(result, str):
                self.matrix_result.insert("end", result)
            else:
                self.matrix_result.insert("end", matrix_to_string(result))

            messagebox.showinfo("Success", "Operation completed!")

        except Exception as e:
            messagebox.showerror("Error", str(e))

    def clear_matrix(self):
        self.matrix_a.delete("1.0", "end")
        self.matrix_b.delete("1.0", "end")
        self.matrix_result.delete("1.0", "end")
        self.matrix_a.insert("1.0", "1 2\n3 4")
        self.matrix_b.insert("1.0", "5 6\n7 8")
        self.update_matrix_size_labels()
if __name__ == "__main__":
    root = tk.Tk()
    app = NumericalMethodsApp(root)
    root.mainloop()
