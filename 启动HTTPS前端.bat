@echo off
chcp 65001 >nul
echo.
echo ========================================
echo   启动 HTTPS 前端服务
echo ========================================
echo.

echo 配置信息:
echo   域名: https://www.a292428dsj.dpdns.org:8443
echo   端口: 8443 (HTTPS)
echo.

echo [1/2] 检查 hosts 文件配置...
findstr /C:"www.a292428dsj.dpdns.org" C:\Windows\System32\drivers\etc\hosts >nul 2>&1
if %errorLevel% neq 0 (
    echo [WARN] hosts 文件未配置域名映射
    echo 建议运行: 配置域名访问.bat (需要管理员权限)
    echo.
) else (
    echo [OK] hosts 文件已配置
    echo.
)

echo [2/2] 启动前端服务...
cd /d "%~dp0src\Frontend\web-app"
echo 正在启动，请稍候...
echo.
echo 访问地址:
echo   https://www.a292428dsj.dpdns.org:8443
echo   https://localhost:8443
echo.
echo 注意:
echo   • 浏览器可能显示证书警告（开发环境正常）
echo   • 点击"高级" → "继续访问"即可
echo   • 如果端口被占用，会自动尝试其他端口
echo.
npm run dev

