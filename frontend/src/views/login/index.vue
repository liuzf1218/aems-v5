<template>
  <div class="login-page">
    <!-- LEFT -->
    <div class="left-panel">
      <canvas ref="particleCanvas" class="particle-canvas"></canvas>
      <div class="left-bg-grid"></div>

      <div class="logo-row">
        <div class="logo-icon-placeholder">⚙</div>
        <div>
          <div class="logo-text">设备管理系统</div>
          <div class="logo-sub">EQUIPMENT MANAGEMENT SYSTEM</div>
        </div>
      </div>

      <div class="radar-wrap">
        <div class="radar-container">
          <svg class="radar-svg" viewBox="0 0 300 300" xmlns="http://www.w3.org/2000/svg">
            <circle class="radar-ring" cx="150" cy="150" r="130"/>
            <circle class="radar-ring" cx="150" cy="150" r="98"/>
            <circle class="radar-ring" cx="150" cy="150" r="65"/>
            <circle class="radar-ring" cx="150" cy="150" r="32"/>
            <line class="radar-cross" x1="20" y1="150" x2="280" y2="150"/>
            <line class="radar-cross" x1="150" y1="20" x2="150" y2="280"/>
            <line class="radar-cross" x1="58" y1="58" x2="242" y2="242"/>
            <line class="radar-cross" x1="242" y1="58" x2="58" y2="242"/>
            <g class="radar-sweep">
              <defs>
                <radialGradient id="sweepGrad" cx="0%" cy="50%" r="100%">
                  <stop offset="0%" stop-color="#409EFF" stop-opacity="0.3"/>
                  <stop offset="100%" stop-color="#409EFF" stop-opacity="0"/>
                </radialGradient>
              </defs>
              <path d="M150,150 L280,150 A130,130 0 0,0 150,20 Z" fill="url(#sweepGrad)" opacity="0.7"/>
              <line x1="150" y1="150" x2="280" y2="150" stroke="#409EFF" stroke-width="1.5" opacity="0.9"/>
            </g>
            <g class="blip">
              <circle cx="195" cy="115" r="4" fill="#67c23a" opacity="0.9"/>
              <circle cx="195" cy="115" r="8" fill="none" stroke="#67c23a" stroke-width="1" opacity="0.4"/>
              <text x="202" y="112" fill="#67c23a" font-size="9" font-family="'Share Tech Mono', monospace" opacity="0.85">服务器</text>
            </g>
            <g class="blip">
              <circle cx="125" cy="82" r="3" fill="#409EFF" opacity="0.9"/>
              <circle cx="125" cy="82" r="6" fill="none" stroke="#409EFF" stroke-width="1" opacity="0.4"/>
              <text x="132" y="79" fill="#409EFF" font-size="9" font-family="'Share Tech Mono', monospace" opacity="0.85">交换机</text>
            </g>
            <g class="blip">
              <circle cx="215" cy="182" r="3.5" fill="#67c23a" opacity="0.9"/>
              <circle cx="215" cy="182" r="7" fill="none" stroke="#67c23a" stroke-width="1" opacity="0.4"/>
              <text x="222" y="179" fill="#67c23a" font-size="9" font-family="'Share Tech Mono', monospace" opacity="0.85">协议转换器</text>
            </g>
            <g class="blip">
              <circle cx="95" cy="170" r="3" fill="#409EFF" opacity="0.9"/>
              <text x="102" y="167" fill="#409EFF" font-size="9" font-family="'Share Tech Mono', monospace" opacity="0.85">气象自动站</text>
            </g>
            <circle cx="150" cy="150" r="4" fill="#409EFF"/>
            <circle cx="150" cy="150" r="8" fill="none" stroke="#409EFF" stroke-width="1" opacity="0.5"/>
          </svg>
        </div>
        <div class="radar-label">设备监控雷达</div>
        <div class="radar-status">
          <div class="pulse-dot"></div>
          SYSTEM ONLINE · 实时追踪中
        </div>
      </div>


    </div>

    <!-- RIGHT -->
    <div class="right-panel">
      <div class="login-card">
        <div class="corner corner-tl"></div>
        <div class="corner corner-br"></div>

        <div class="card-header">
          <div class="card-logo-placeholder">⚙</div>
          <div class="card-title">系统登录</div>
          <div class="card-sub">设备管理平台 V5.0</div>
        </div>

        <el-form
          ref="formRef"
          :model="loginForm"
          :rules="loginRules"
          class="login-form"
          @keyup.enter="handleLogin"
        >
          <el-form-item prop="username">
            <div class="field-label">用户名</div>
            <div class="field-wrap">
              <span class="field-icon">
                <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5">
                  <path d="M20 21v-2a4 4 0 0 0-4-4H8a4 4 0 0 0-4 4v2"/><circle cx="12" cy="7" r="4"/>
                </svg>
              </span>
              <input
                v-model="loginForm.username"
                class="field-input"
                type="text"
                placeholder="请输入用户名"
                @blur="formRef?.validateField('username')"
              />
            </div>
          </el-form-item>

          <el-form-item prop="password">
            <div class="field-label">密码</div>
            <div class="field-wrap">
              <span class="field-icon">
                <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5">
                  <rect x="3" y="11" width="18" height="11" rx="2"/><path d="M7 11V7a5 5 0 0 1 10 0v4"/>
                </svg>
              </span>
              <input
                v-model="loginForm.password"
                class="field-input"
                type="password"
                placeholder="请输入密码"
                @blur="formRef?.validateField('password')"
              />
            </div>
          </el-form-item>

          <el-form-item>
            <div class="options-row">
              <label class="checkbox-wrap">
                <input v-model="rememberMe" type="checkbox" />
                <span class="custom-check"></span>
                <span class="check-label">记住密码</span>
              </label>
              <a href="#" class="forgot" @click.prevent>忘记密码？</a>
            </div>
          </el-form-item>

          <el-form-item>
            <button type="button" class="submit-btn" :disabled="loading" @click="handleLogin">
              {{ loading ? '登录中 ...' : '登 录' }}
            </button>
          </el-form-item>
        </el-form>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted, onUnmounted } from 'vue'
import { useRouter } from 'vue-router'
import { ElMessage } from 'element-plus'
import { useUserStore } from '@/store/user'

const router = useRouter()
const userStore = useUserStore()
const formRef = ref(null)
const loading = ref(false)
const rememberMe = ref(false)
const particleCanvas = ref(null)

const loginForm = reactive({
  username: localStorage.getItem('aems_remember_user') || '',
  password: ''
})

const loginRules = {
  username: [
    { required: true, message: '请输入用户名', trigger: 'blur' },
    { min: 2, max: 20, message: '长度在 2 到 20 个字符', trigger: 'blur' }
  ],
  password: [
    { required: true, message: '请输入密码', trigger: 'blur' },
    { min: 4, max: 30, message: '长度在 4 到 30 个字符', trigger: 'blur' }
  ]
}

const handleLogin = async () => {
  const valid = await formRef.value?.validate().catch(() => false)
  if (!valid) return
  loading.value = true
  try {
    await userStore.login({ username: loginForm.username, password: loginForm.password })
    if (rememberMe.value) {
      localStorage.setItem('aems_remember_user', loginForm.username)
    } else {
      localStorage.removeItem('aems_remember_user')
    }
    ElMessage.success('登录成功')
    router.push('/dashboard')
  } catch (err) {
    ElMessage.error('登录失败，请检查用户名和密码')
  } finally {
    loading.value = false
  }
}

/* ========== Particle Animation ========== */
let animId = null

onMounted(() => {
  const canvas = particleCanvas.value
  if (!canvas) return
  const ctx = canvas.getContext('2d')
  const leftPanel = canvas.parentElement

  function resizeCanvas() {
    canvas.width = leftPanel.offsetWidth
    canvas.height = leftPanel.offsetHeight
  }
  resizeCanvas()
  window.addEventListener('resize', resizeCanvas)

  const particles = Array.from({ length: 50 }, () => ({
    x: Math.random() * canvas.width,
    y: Math.random() * canvas.height,
    r: Math.random() * 1.2 + 0.2,
    vx: (Math.random() - 0.5) * 0.2,
    vy: (Math.random() - 0.5) * 0.2,
    a: Math.random() * 0.35 + 0.1,
    color: Math.random() > 0.5 ? '64,158,255' : '103,194,58'
  }))

  const runways = [
    { x1: 0.05, y1: 0.55, x2: 0.45, y2: 0.48, progress: 0 },
    { x1: 0.08, y1: 0.65, x2: 0.45, y2: 0.6, progress: 0.3 }
  ]

  function draw() {
    const W = canvas.width, H = canvas.height
    ctx.clearRect(0, 0, W, H)

    runways.forEach(r => {
      r.progress = (r.progress + 0.0015) % 1
      const x1 = r.x1 * W, y1 = r.y1 * H, x2 = r.x2 * W, y2 = r.y2 * H
      ctx.save()
      ctx.strokeStyle = 'rgba(64,158,255,0.15)'
      ctx.lineWidth = 1
      ctx.setLineDash([20, 15])
      ctx.lineDashOffset = -r.progress * 350
      ctx.beginPath(); ctx.moveTo(x1, y1); ctx.lineTo(x2, y2); ctx.stroke()
      ctx.restore()
      const px = x1 + (x2 - x1) * r.progress, py = y1 + (y2 - y1) * r.progress
      const lg = ctx.createRadialGradient(px, py, 0, px, py, 12)
      lg.addColorStop(0, 'rgba(64,158,255,0.5)')
      lg.addColorStop(1, 'rgba(64,158,255,0)')
      ctx.beginPath(); ctx.arc(px, py, 12, 0, Math.PI * 2)
      ctx.fillStyle = lg; ctx.fill()
    })

    particles.forEach(p => {
      p.x += p.vx; p.y += p.vy
      if (p.x < 0) p.x = W; if (p.x > W) p.x = 0
      if (p.y < 0) p.y = H; if (p.y > H) p.y = 0
      ctx.beginPath(); ctx.arc(p.x, p.y, p.r, 0, Math.PI * 2)
      ctx.fillStyle = `rgba(${p.color},${p.a})`; ctx.fill()
    })

    particles.forEach((a, i) => {
      particles.slice(i + 1).forEach(b => {
        const dist = Math.hypot(a.x - b.x, a.y - b.y)
        if (dist < 80) {
          ctx.beginPath(); ctx.moveTo(a.x, a.y); ctx.lineTo(b.x, b.y)
          ctx.strokeStyle = `rgba(64,158,255,${0.05 * (1 - dist / 80)})`
          ctx.lineWidth = 0.5; ctx.stroke()
        }
      })
    })

    animId = requestAnimationFrame(draw)
  }
  draw()

  onUnmounted(() => {
    window.removeEventListener('resize', resizeCanvas)
    if (animId) cancelAnimationFrame(animId)
  })
})
</script>

<style scoped lang="scss">
.login-page {
  display: grid;
  grid-template-columns: 3fr 2fr;
  height: 100vh;
  overflow: hidden;
}

/* ========== LEFT ========== */
.left-panel {
  position: relative;
  display: flex;
  flex-direction: column;
  justify-content: space-between;
  padding: 48px 56px;
  background: linear-gradient(135deg, #0c1c40 0%, #0a1428 100%);
  color: #e6eef8;
  overflow: hidden;
  animation: fadeUp 0.8s ease both;
}

.left-bg-grid {
  position: absolute; inset: 0; z-index: 0;
  background-image:
    linear-gradient(rgba(64,158,255,0.04) 1px, transparent 1px),
    linear-gradient(90deg, rgba(64,158,255,0.04) 1px, transparent 1px);
  background-size: 56px 56px;
  pointer-events: none;
}

.particle-canvas {
  position: absolute; inset: 0; z-index: 1; pointer-events: none;
}

.logo-row {
  position: relative; z-index: 2;
  display: flex; align-items: center; gap: 14px;
}
.logo-icon-placeholder { width: 44px; height: 44px; flex-shrink: 0; border-radius: 8px; background: linear-gradient(135deg, #409EFF, #67c23a); display: flex; align-items: center; justify-content: center; font-size: 24px; color: #fff; }
.logo-text {
  font-family: 'Bebas Neue', sans-serif;
  font-size: 26px; letter-spacing: 2px; color: #fff; line-height: 1.1;
}
.logo-sub {
  font-family: 'Share Tech Mono', monospace; font-size: 9px; color: rgba(255,255,255,0.45);
  letter-spacing: 1.5px; margin-top: 3px;
}

.radar-wrap {
  position: relative; z-index: 2;
  display: flex; flex-direction: column; align-items: center; gap: 22px;
  flex: 1; justify-content: center;
}

.radar-container { position: relative; width: 420px; height: 420px; }
.radar-svg { width: 100%; height: 100%; }
.radar-ring { fill: none; stroke: rgba(64,158,255,0.18); stroke-width: 1; }
.radar-cross { stroke: rgba(64,158,255,0.12); stroke-width: 1; }
.radar-sweep { transform-origin: 150px 150px; animation: sweep 4s linear infinite; }
@keyframes sweep { from { transform: rotate(0deg); } to { transform: rotate(360deg); } }

.blip { animation: blipFade 4s linear infinite; }
.blip:nth-child(2) { animation-delay: -1.1s; }
.blip:nth-child(3) { animation-delay: -2.3s; }
.blip:nth-child(4) { animation-delay: -3.1s; }
@keyframes blipFade {
  0%,100% { opacity: 0; }
  20%, 40% { opacity: 1; }
  80% { opacity: 0.3; }
}

.radar-label {
  font-family: 'Share Tech Mono', monospace; font-size: 13px; color: rgba(255,255,255,0.4);
  letter-spacing: 2px; text-align: center;
}
.radar-status {
  display: flex; align-items: center; gap: 8px;
  font-family: 'Share Tech Mono', monospace; font-size: 13px; color: #67c23a; letter-spacing: 1px;
}
.pulse-dot {
  width: 8px; height: 8px; border-radius: 50%; background: #67c23a;
  box-shadow: 0 0 10px #67c23a; animation: pulseDot 1.4s ease-in-out infinite;
}
@keyframes pulseDot {
  0%,100% { transform: scale(1); opacity: 1; }
  50% { transform: scale(1.5); opacity: 0.5; }
}

/* ========== RIGHT ========== */
.right-panel {
  position: relative;
  display: flex; flex-direction: column; justify-content: center; align-items: center;
  padding: 40px 48px;
  background: linear-gradient(135deg, #1a2a4a 0%, #0d1b2a 100%);
  animation: fadeUp 0.8s ease 0.1s both;
}

.right-panel::before {
  content: ''; position: absolute; inset: 0;
  background: linear-gradient(to bottom, rgba(0,0,0,0.1) 0%, rgba(0,0,0,0.25) 100%);
  pointer-events: none;
}

.login-card {
  position: relative; z-index: 1;
  width: 100%; max-width: 360px;
  background: rgba(255,255,255,0.97);
  border-radius: 16px;
  padding: 32px 28px;
  box-shadow: 0 20px 60px rgba(0,0,0,0.25), 0 0 0 1px rgba(255,255,255,0.3) inset;
  backdrop-filter: blur(12px);
}

.card-header { text-align: center; margin-bottom: 24px; }
.card-logo-placeholder { width: 52px; height: 52px; margin-bottom: 8px; background: linear-gradient(135deg, #409EFF, #67c23a); border-radius: 12px; display: flex; align-items: center; justify-content: center; font-size: 28px; color: #fff; }
.card-title { font-size: 20px; font-weight: 700; color: #303133; letter-spacing: 2px; margin-bottom: 4px; }
.card-sub { font-size: 12px; color: #606266; letter-spacing: 0.5px; }

.login-form {
  display: flex; flex-direction: column; gap: 18px;
}
:deep(.el-form-item) {
  margin-bottom: 0;
}
:deep(.el-form-item__content) {
  display: block;
  line-height: normal;
}

.field-label {
  font-size: 12px; color: #606266; font-weight: 500; letter-spacing: 0.5px; margin-bottom: 6px;
}
.field-wrap { position: relative; display: flex; align-items: center; }
.field-icon {
  position: absolute; left: 14px; color: #a8abb2;
  pointer-events: none; display: flex; z-index: 2;
}
.field-input {
  width: 100%; height: 40px;
  background: #fff;
  border: 1px solid #dcdfe6;
  border-radius: 8px;
  padding: 0 14px 0 40px;
  font-size: 14px; color: #303133;
  outline: none; transition: all 0.2s;
}
.field-input::placeholder { color: #a8abb2; }
.field-input:focus {
  border-color: #409EFF;
  box-shadow: 0 0 0 3px rgba(64,158,255,0.15);
}

.options-row {
  display: flex; justify-content: space-between; align-items: center; margin-top: 2px;
}
.checkbox-wrap {
  display: flex; align-items: center; gap: 8px; cursor: pointer;
}
.checkbox-wrap input[type=checkbox] { display: none; }
.custom-check {
  width: 16px; height: 16px;
  border: 1px solid #dcdfe6; border-radius: 4px;
  background: #fff; display: flex; align-items: center; justify-content: center;
  transition: all 0.2s; flex-shrink: 0;
}
.checkbox-wrap input:checked + .custom-check {
  background: #409EFF; border-color: #409EFF;
}
.checkbox-wrap input:checked + .custom-check::after {
  content: ''; width: 4px; height: 7px;
  border-right: 2px solid #fff; border-bottom: 2px solid #fff;
  transform: rotate(45deg) translate(-1px,-1px); display: block;
}
.check-label { font-size: 13px; color: #606266; user-select: none; }
.forgot {
  font-size: 13px; color: #606266; text-decoration: none;
  transition: color 0.2s;
}
.forgot:hover { color: #409EFF; }

.submit-btn {
  width: 100%; height: 42px;
  background: #409EFF; border: none; border-radius: 8px;
  color: #fff; font-size: 16px; font-weight: 500;
  letter-spacing: 4px; cursor: pointer;
  transition: all 0.25s; margin-top: 4px;
  box-shadow: 0 4px 14px rgba(64,158,255,0.35);
}
.submit-btn:hover {
  background: #66b1ff;
  box-shadow: 0 6px 20px rgba(64,158,255,0.45);
  transform: translateY(-1px);
}
.submit-btn:active { transform: scale(0.99); }
.submit-btn:disabled {
  opacity: 0.7; cursor: not-allowed; transform: none;
}

.corner {
  position: absolute; width: 20px; height: 20px; pointer-events: none; z-index: 2;
}
.corner-tl { top: 16px; left: 16px; border-top: 2px solid #409EFF; border-left: 2px solid #409EFF; border-radius: 2px 0 0 0; }
.corner-br { bottom: 16px; right: 16px; border-bottom: 2px solid #409EFF; border-right: 2px solid #409EFF; border-radius: 0 0 2px 0; }

@keyframes fadeUp {
  from { opacity: 0; transform: translateY(20px); }
  to   { opacity: 1; transform: translateY(0); }
}

/* responsive */
@media (max-width: 960px) {
  .login-page { grid-template-columns: 1fr; }
  .left-panel { display: none; }
  .right-panel { background-position: center center; padding: 32px 24px; }
}

/* el-form error message style override */
:deep(.el-form-item__error) {
  padding-top: 4px;
  font-size: 12px;
}
</style>
