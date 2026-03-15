// ─── Constants ────────────────────────────────────────────────────────────────
const CANVAS_W = 480;
const CANVAS_H = 640;

const PLAYER_W = 40;
const PLAYER_H = 32;
const PLAYER_SPEED = 5;
const BULLET_W = 4;
const BULLET_H = 14;
const BULLET_SPEED = 8;
const ENEMY_W = 36;
const ENEMY_H = 28;
const ENEMY_COLS = 8;
const ENEMY_ROWS = 4;
const ENEMY_PADDING = 12;
const ENEMY_OFFSET_X = 30;
const ENEMY_OFFSET_Y = 50;
const ENEMY_DROP = 18;
const SHOOT_COOLDOWN = 18; // frames between shots

// ─── Game State ───────────────────────────────────────────────────────────────
const canvas = document.getElementById('gameCanvas');
const ctx = canvas.getContext('2d');
const overlay = document.getElementById('overlay');
const overlayTitle = document.getElementById('overlay-title');
const overlayMessage = document.getElementById('overlay-message');
const finalScore = document.getElementById('final-score');
const startBtn = document.getElementById('start-btn');

let player, bullets, enemies, score, lives, gameRunning, animId;
let keys = {};
let shootCooldown = 0;

// ─── Initialise ───────────────────────────────────────────────────────────────
function initGame() {
  player = {
    x: CANVAS_W / 2 - PLAYER_W / 2,
    y: CANVAS_H - PLAYER_H - 16,
    w: PLAYER_W,
    h: PLAYER_H,
    speed: PLAYER_SPEED,
  };
  bullets = [];
  enemies = createEnemies();
  score = 0;
  lives = 3;
  shootCooldown = 0;
  gameRunning = true;

  overlay.classList.add('hidden');
  if (animId) cancelAnimationFrame(animId);
  gameLoop();
}

function createEnemies() {
  const list = [];
  let direction = 1; // 1 = right, -1 = left
  let speed = 1;

  // All enemies share movement state via closure
  for (let row = 0; row < ENEMY_ROWS; row++) {
    for (let col = 0; col < ENEMY_COLS; col++) {
      list.push({
        x: ENEMY_OFFSET_X + col * (ENEMY_W + ENEMY_PADDING),
        y: ENEMY_OFFSET_Y + row * (ENEMY_H + ENEMY_PADDING),
        w: ENEMY_W,
        h: ENEMY_H,
        alive: true,
        row,
      });
    }
  }

  // Attach shared movement state to list
  list.direction = direction;
  list.speed = speed;
  return list;
}

// ─── Update ───────────────────────────────────────────────────────────────────
function update() {
  // Player movement
  if ((keys['ArrowLeft'] || keys['a'] || keys['A']) && player.x > 0) {
    player.x -= player.speed;
  }
  if ((keys['ArrowRight'] || keys['d'] || keys['D']) && player.x + player.w < CANVAS_W) {
    player.x += player.speed;
  }

  // Shooting
  if (shootCooldown > 0) shootCooldown--;
  if ((keys[' '] || keys['Space']) && shootCooldown === 0) {
    bullets.push({
      x: player.x + player.w / 2 - BULLET_W / 2,
      y: player.y,
      w: BULLET_W,
      h: BULLET_H,
    });
    shootCooldown = SHOOT_COOLDOWN;
  }

  // Move bullets
  bullets = bullets.filter(b => b.enemy ? b.y < CANVAS_H : b.y + b.h > 0);
  bullets.forEach(b => (b.y += b.enemy ? BULLET_SPEED : -BULLET_SPEED));

  // Move enemies
  const alive = enemies.filter(e => e.alive);
  if (alive.length === 0) {
    // Next wave — speed up, preserving current speed
    const nextSpeed = Math.min(enemies.speed + 0.5, 4);
    enemies = createEnemies();
    enemies.speed = nextSpeed;
    return;
  }

  let hitWall = false;
  alive.forEach(e => {
    e.x += enemies.direction * enemies.speed;
  });

  const minX = Math.min(...alive.map(e => e.x));
  const maxX = Math.max(...alive.map(e => e.x + e.w));
  if (maxX >= CANVAS_W || minX <= 0) hitWall = true;

  if (hitWall) {
    enemies.direction *= -1;
    alive.forEach(e => (e.y += ENEMY_DROP));
  }

  // Enemy random shooting
  if (Math.random() < 0.015 && alive.length > 0) {
    const shooter = alive[Math.floor(Math.random() * alive.length)];
    bullets.push({
      x: shooter.x + shooter.w / 2 - BULLET_W / 2,
      y: shooter.y + shooter.h,
      w: BULLET_W,
      h: BULLET_H,
      enemy: true,
    });
  }

  // Bullet ↔ enemy collision
  bullets.forEach(b => {
    if (b.enemy) return;
    enemies.forEach(e => {
      if (e.alive && rectsOverlap(b, e)) {
        e.alive = false;
        b.hit = true;
        score += 10;
      }
    });
  });
  bullets = bullets.filter(b => !b.hit);

  // Enemy bullet ↔ player collision
  bullets.forEach(b => {
    if (!b.enemy) return;
    if (rectsOverlap(b, player)) {
      b.hit = true;
      lives--;
      if (lives <= 0) endGame(false);
    }
  });
  bullets = bullets.filter(b => !b.hit);

  // Enemy reaches bottom / player
  alive.forEach(e => {
    if (e.y + e.h >= player.y) endGame(false);
  });
}

// ─── Draw ─────────────────────────────────────────────────────────────────────
function draw() {
  ctx.clearRect(0, 0, CANVAS_W, CANVAS_H);

  // Stars background
  drawStars();

  // Player ship
  drawPlayer();

  // Bullets
  bullets.forEach(b => {
    ctx.fillStyle = b.enemy ? '#f55' : '#4ef';
    ctx.shadowColor = b.enemy ? '#f55' : '#4ef';
    ctx.shadowBlur = 8;
    ctx.fillRect(b.x, b.y, b.w, b.h);
    ctx.shadowBlur = 0;
  });

  // Enemies
  enemies.forEach(e => {
    if (e.alive) drawEnemy(e);
  });

  // HUD
  ctx.fillStyle = '#fff';
  ctx.font = 'bold 16px monospace';
  ctx.fillText(`Score: ${score}`, 10, 24);
  ctx.fillText(`Lives: ${'♥ '.repeat(lives).trim()}`, CANVAS_W - 110, 24);
}

let stars = null;
function drawStars() {
  if (!stars) {
    stars = Array.from({ length: 60 }, () => ({
      x: Math.random() * CANVAS_W,
      y: Math.random() * CANVAS_H,
      r: Math.random() * 1.5 + 0.3,
      a: Math.random() * 0.7 + 0.3,
    }));
  }
  stars.forEach(s => {
    ctx.beginPath();
    ctx.arc(s.x, s.y, s.r, 0, Math.PI * 2);
    ctx.fillStyle = `rgba(255,255,255,${s.a})`;
    ctx.fill();
  });
}

function drawPlayer() {
  const { x, y, w, h } = player;
  ctx.save();

  // Body
  ctx.fillStyle = '#4af';
  ctx.shadowColor = '#4af';
  ctx.shadowBlur = 12;
  ctx.beginPath();
  ctx.moveTo(x + w / 2, y);
  ctx.lineTo(x + w, y + h);
  ctx.lineTo(x, y + h);
  ctx.closePath();
  ctx.fill();

  // Cockpit
  ctx.fillStyle = '#aef';
  ctx.beginPath();
  ctx.ellipse(x + w / 2, y + h * 0.55, w * 0.15, h * 0.22, 0, 0, Math.PI * 2);
  ctx.fill();

  ctx.shadowBlur = 0;
  ctx.restore();
}

function drawEnemy(e) {
  const { x, y, w, h, row } = e;
  const colors = ['#f55', '#fa0', '#af0', '#0af'];
  const color = colors[row % colors.length];

  ctx.save();
  ctx.fillStyle = color;
  ctx.shadowColor = color;
  ctx.shadowBlur = 8;

  // Body: saucer shape
  ctx.beginPath();
  ctx.ellipse(x + w / 2, y + h * 0.6, w * 0.5, h * 0.38, 0, 0, Math.PI * 2);
  ctx.fill();

  // Dome
  ctx.fillStyle = '#fff8';
  ctx.beginPath();
  ctx.ellipse(x + w / 2, y + h * 0.42, w * 0.28, h * 0.25, 0, 0, Math.PI * 2);
  ctx.fill();

  // Legs
  ctx.fillStyle = color;
  ctx.fillRect(x + w * 0.1, y + h * 0.78, w * 0.12, h * 0.22);
  ctx.fillRect(x + w * 0.44, y + h * 0.78, w * 0.12, h * 0.22);
  ctx.fillRect(x + w * 0.78, y + h * 0.78, w * 0.12, h * 0.22);

  ctx.shadowBlur = 0;
  ctx.restore();
}

// ─── Helpers ──────────────────────────────────────────────────────────────────
function rectsOverlap(a, b) {
  return (
    a.x < b.x + b.w &&
    a.x + a.w > b.x &&
    a.y < b.y + b.h &&
    a.y + a.h > b.y
  );
}

function endGame(won) {
  gameRunning = false;
  cancelAnimationFrame(animId);
  overlayTitle.textContent = won ? '🎉 You Win!' : '💀 Game Over';
  overlayMessage.textContent = won
    ? 'Congratulations, you destroyed all invaders!'
    : 'The invaders have won this time...';
  finalScore.textContent = `Final Score: ${score}`;
  startBtn.textContent = 'Play Again';
  overlay.classList.remove('hidden');
}

// ─── Game Loop ────────────────────────────────────────────────────────────────
function gameLoop() {
  if (!gameRunning) return;
  update();
  draw();
  animId = requestAnimationFrame(gameLoop);
}

// ─── Input ────────────────────────────────────────────────────────────────────
window.addEventListener('keydown', e => {
  keys[e.key] = true;
  if (e.key === ' ') e.preventDefault();
});
window.addEventListener('keyup', e => {
  keys[e.key] = false;
});

// ─── Start Screen ─────────────────────────────────────────────────────────────
startBtn.addEventListener('click', initGame);

// Show start overlay initially
overlayTitle.textContent = 'Space Shooter';
overlayMessage.textContent = 'Destroy the invaders before they reach you!';
finalScore.textContent = '';
startBtn.textContent = 'Start Game';
overlay.classList.remove('hidden');
