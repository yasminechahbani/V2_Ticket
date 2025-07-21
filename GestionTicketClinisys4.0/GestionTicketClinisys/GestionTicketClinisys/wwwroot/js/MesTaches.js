// MesTaches.js - JavaScript for the user tasks page

// Initialize when DOM is loaded
document.addEventListener("DOMContentLoaded", function () {
  initializeFilters();
  initializeEventListeners();
});

// Update task status
function updateTaskStatus(taskId, newStatus) {
  // Add loading state
  const row = document.querySelector(`tr[data-task-id="${taskId}"]`);
  const select = row.querySelector(".status-select");
  const originalValue = select.value;

  // Disable select during update
  select.disabled = true;

  fetch("/Tasks/UpdateTaskStatus", {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify({
      taskId: taskId,
      status: newStatus,
    }),
  })
    .then((response) => response.json())
    .then((data) => {
      if (data.success) {
        // Update the status badge
        const statusBadge = row.querySelector(".status-badge");
        statusBadge.className = `status-badge status-${newStatus.toLowerCase()}`;
        statusBadge.textContent = getStatusDisplayName(newStatus);

        showNotification("Statut mis à jour avec succès", "success");
      } else {
        // Revert select to original value
        select.value = originalValue;
        showNotification(
          data.message || "Erreur lors de la mise à jour",
          "error"
        );
      }
    })
    .catch((error) => {
      console.error("Error:", error);
      // Revert select to original value
      select.value = originalValue;
      showNotification("Erreur lors de la mise à jour", "error");
    })
    .finally(() => {
      // Re-enable select
      select.disabled = false;
    });
}

// Get display name for status
function getStatusDisplayName(status) {
  const statusMap = {
    NotStarted: "Non commencé",
    InProgress: "En cours",
    Completed: "Terminé",
    Cancelled: "Annulé",
  };
  return statusMap[status] || status;
}

// Initialize filter functionality
function initializeFilters() {
  const statusFilter = document.getElementById("statusFilter");
  const searchInput = document.getElementById("searchInput");

  if (statusFilter) {
    statusFilter.addEventListener("change", filterTasks);
  }

  if (searchInput) {
    searchInput.addEventListener("input", debounce(filterTasks, 300));
  }
}

// Initialize other event listeners
function initializeEventListeners() {
  // Add keyboard shortcuts
  document.addEventListener("keydown", function (e) {
    // Ctrl+F to focus search
    if (e.ctrlKey && e.key === "f") {
      e.preventDefault();
      const searchInput = document.getElementById("searchInput");
      if (searchInput) {
        searchInput.focus();
      }
    }

    // Escape to clear search
    if (e.key === "Escape") {
      const searchInput = document.getElementById("searchInput");
      if (searchInput && searchInput === document.activeElement) {
        searchInput.value = "";
        filterTasks();
      }
    }
  });
}

// Filter tasks based on status and search term
function filterTasks() {
  const statusFilter = document.getElementById("statusFilter");
  const searchInput = document.getElementById("searchInput");

  const selectedStatus = statusFilter ? statusFilter.value : "";
  const searchTerm = searchInput ? searchInput.value.toLowerCase() : "";

  const rows = document.querySelectorAll("#tasksTable tbody tr");
  let visibleCount = 0;

  rows.forEach((row) => {
    // Skip empty state row
    if (row.querySelector(".empty-state")) {
      return;
    }

    const statusSelect = row.querySelector(".status-select");
    const currentStatus = statusSelect ? statusSelect.value : "";

    // Get text content for search
    const title = row.cells[0] ? row.cells[0].textContent.toLowerCase() : "";
    const description = row.cells[1]
      ? row.cells[1].textContent.toLowerCase()
      : "";
    const ticket = row.cells[2] ? row.cells[2].textContent.toLowerCase() : "";
    const client = row.cells[3] ? row.cells[3].textContent.toLowerCase() : "";

    // Check filters
    const matchesStatus = !selectedStatus || currentStatus === selectedStatus;
    const matchesSearch =
      !searchTerm ||
      title.includes(searchTerm) ||
      description.includes(searchTerm) ||
      ticket.includes(searchTerm) ||
      client.includes(searchTerm);

    const shouldShow = matchesStatus && matchesSearch;
    row.style.display = shouldShow ? "" : "none";

    if (shouldShow) {
      visibleCount++;
    }
  });

  // Update empty state visibility
  updateEmptyState(visibleCount);
}

// Update empty state based on visible tasks
function updateEmptyState(visibleCount) {
  const emptyStateRow = document.querySelector(
    "#tasksTable tbody tr .empty-state"
  );
  const hasData =
    document.querySelectorAll(
      '#tasksTable tbody tr:not([style*="display: none"])'
    ).length > 1;

  if (emptyStateRow) {
    const emptyRow = emptyStateRow.closest("tr");
    if (visibleCount === 0 && hasData) {
      // Show "no results" message
      emptyStateRow.innerHTML = `
                <i class="fas fa-search fa-3x"></i>
                <h3>Aucun résultat trouvé</h3>
                <p>Aucune tâche ne correspond à vos critères de recherche.</p>
            `;
      emptyRow.style.display = "";
    } else if (visibleCount === 0 && !hasData) {
      // Show "no tasks" message
      emptyStateRow.innerHTML = `
                <i class="fas fa-tasks fa-3x"></i>
                <h3>Aucune tâche assignée</h3>
                <p>Vous n'avez actuellement aucune tâche assignée.</p>
            `;
      emptyRow.style.display = "";
    } else {
      emptyRow.style.display = "none";
    }
  }
}

// Show notification to user
function showNotification(message, type) {
  // Remove existing notifications
  const existingNotifications = document.querySelectorAll(".notification");
  existingNotifications.forEach((notification) => notification.remove());

  // Create new notification element
  const notification = document.createElement("div");
  notification.className = `notification ${type}`;
  notification.textContent = message;

  // Add close button
  const closeButton = document.createElement("span");
  closeButton.innerHTML = "&times;";
  closeButton.style.cssText = `
        float: right;
        margin-left: 10px;
        cursor: pointer;
        font-size: 18px;
        line-height: 1;
    `;
  closeButton.onclick = () => notification.remove();
  notification.appendChild(closeButton);

  document.body.appendChild(notification);

  // Auto-remove notification after 5 seconds
  setTimeout(() => {
    if (notification.parentNode) {
      notification.remove();
    }
  }, 5000);
}

// Debounce function to limit search frequency
function debounce(func, wait) {
  let timeout;
  return function executedFunction(...args) {
    const later = () => {
      clearTimeout(timeout);
      func(...args);
    };
    clearTimeout(timeout);
    timeout = setTimeout(later, wait);
  };
}

// Refresh tasks data
function refreshTasks() {
  fetch("/Tasks/GetUserTasks")
    .then((response) => response.json())
    .then((data) => {
      if (data.success) {
        updateTasksTable(data.data);
        showNotification("Tâches actualisées", "success");
      } else {
        showNotification(
          data.message || "Erreur lors de l'actualisation",
          "error"
        );
      }
    })
    .catch((error) => {
      console.error("Error:", error);
      showNotification("Erreur lors de l'actualisation", "error");
    });
}

// Update tasks table with new data
function updateTasksTable(tasks) {
  const tbody = document.querySelector("#tasksTable tbody");
  if (!tbody) return;

  // Clear existing rows
  tbody.innerHTML = "";

  if (tasks && tasks.length > 0) {
    tasks.forEach((task) => {
      const row = createTaskRow(task);
      tbody.appendChild(row);
    });
  } else {
    // Add empty state row
    const emptyRow = document.createElement("tr");
    emptyRow.innerHTML = `
            <td colspan="7" class="text-center">
                <div class="empty-state">
                    <i class="fas fa-tasks fa-3x"></i>
                    <h3>Aucune tâche assignée</h3>
                    <p>Vous n'avez actuellement aucune tâche assignée.</p>
                </div>
            </td>
        `;
    tbody.appendChild(emptyRow);
  }

  // Reapply filters
  filterTasks();
}

// Create a task row element
function createTaskRow(task) {
  const row = document.createElement("tr");
  row.setAttribute("data-task-id", task.id);

  row.innerHTML = `
        <td><strong>${escapeHtml(task.title)}</strong></td>
        <td>
            <div class="description-cell">
                ${escapeHtml(
                  task.description.length > 100
                    ? task.description.substring(0, 100) + "..."
                    : task.description
                )}
            </div>
        </td>
        <td>
            <a href="/Tickets/GestionDemandes" class="ticket-link">
                ${escapeHtml(task.ticketTitle)}
            </a>
        </td>
        <td>${escapeHtml(task.clientName)}</td>
        <td>
            <span class="status-badge status-${task.status.toLowerCase()}">
                ${getStatusDisplayName(task.status)}
            </span>
        </td>
        <td>${task.createdAt}</td>
        <td>
            <div class="action-buttons">
                <select class="status-select" data-task-id="${
                  task.id
                }" onchange="updateTaskStatus(${task.id}, this.value)">
                    <option value="NotStarted" ${
                      task.status === "NotStarted" ? "selected" : ""
                    }>Non commencé</option>
                    <option value="InProgress" ${
                      task.status === "InProgress" ? "selected" : ""
                    }>En cours</option>
                    <option value="Completed" ${
                      task.status === "Completed" ? "selected" : ""
                    }>Terminé</option>
                    <option value="Cancelled" ${
                      task.status === "Cancelled" ? "selected" : ""
                    }>Annulé</option>
                </select>
            </div>
        </td>
    `;

  return row;
}

// Escape HTML to prevent XSS
function escapeHtml(text) {
  const map = {
    "&": "&amp;",
    "<": "&lt;",
    ">": "&gt;",
    '"': "&quot;",
    "'": "&#039;",
  };
  return text.replace(/[&<>"']/g, function (m) {
    return map[m];
  });
}

// Clear all filters
function clearFilters() {
  document.getElementById("searchInput").value = "";
  document.getElementById("statusFilter").value = "";
  filterTasks();
}

// Export functions for global access
window.updateTaskStatus = updateTaskStatus;
window.refreshTasks = refreshTasks;
window.clearFilters = clearFilters;
