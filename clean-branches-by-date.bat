@echo off
SETLOCAL ENABLEDELAYEDEXPANSION

:: ========================================
:: Script: clean-branches-by-date.bat
:: Autor: ChatGPT & Senpai (2025 Edition)
:: Objetivo: apagar branches locais e remotas por data
:: ========================================

:: === Configurações iniciais ===
set DATE_TARGET=2025-10-11
echo 🧹 Limpando branches com data: %DATE_TARGET%
echo.

:: === Confere se é um repositório Git ===
git rev-parse --is-inside-work-tree >nul 2>&1
IF ERRORLEVEL 1 (
    echo ❌ Este diretório nao é um repositório Git.
    EXIT /B 1
)

:: === Garante que não estamos numa branch que será apagada ===
git checkout main >nul 2>&1

:: === Listar branches locais correspondentes ===
echo 🔍 Buscando branches locais alteradas em %DATE_TARGET%...
FOR /F "tokens=1 delims= " %%b IN ('git for-each-ref --sort=-committerdate --format="%%(refname:short) %%(committerdate:short)" refs/heads/ ^| find "%DATE_TARGET%"') DO (
    echo 🗑️  Deletando local: %%b
    git branch -D %%b >nul 2>&1
)

:: === Buscar e apagar branches remotas ===
echo.
echo 🌐 Limpando branches remotas alteradas em %DATE_TARGET%...
FOR /F "tokens=1 delims= " %%r IN ('git for-each-ref --format="%%(refname:short) %%(committerdate:short)" refs/remotes/origin ^| find "%DATE_TARGET%"') DO (
    set BRANCH=%%r
    set BRANCH=!BRANCH:origin/=!
    echo 💀 Deletando remoto: !BRANCH!
    git push origin --delete !BRANCH! >nul 2>&1
)

:: === Atualiza referências locais ===
echo.
git fetch origin --prune >nul 2>&1
echo ✅ Branches antigas eliminadas com sucesso.
echo.
echo 🧾 Finalizado. Tudo limpo e sincronizado com o remoto.

ENDLOCAL
pause
