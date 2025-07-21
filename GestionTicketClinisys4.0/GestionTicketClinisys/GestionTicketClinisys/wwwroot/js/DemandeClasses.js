// DemandeClasses.js - JavaScript for the task management page

// Initialize when DOM is loaded
document.addEventListener("DOMContentLoaded", function () {
  initializeFilters();
  initializeEventListeners();
  loadUsersForFilter();
});

// Initialize filter functionality
function initializeFilters() {
  const statusFilter = document.getElementById("statusFilter");
  const userFilter = document.getElementById("userFilter");
  const searchInput = document.getElementById("searchInput");

  if (statusFilter) {
    statusFilter.addEventListener("change", applyFilters);
  }

  if (userFilter) {
    userFilter.addEventListener("change", applyFilters);
  }

  if (searchInput) {
    searchInput.addEventListener("input", debounce(applyFilters, 300));
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

    // Escape to close modals
    if (e.key === "Escape") {
      closeCreateTaskModal();
      closeEditTaskModal();
    }
  });

  // Checkbox selection handling
  updateDeleteButtonState();
  document.addEventListener("change", function (e) {
    if (
      e.target.classList.contains("task-checkbox") ||
      e.target.id === "selectAll"
    ) {
      updateDeleteButtonState();
    }
  });
}

// Load users for filter dropdown
async function loadUsersForFilter() {
  try {
    const response = await fetch("/DemandeClasses/GetUsersForAssignment");
    const result = await response.json();

    if (result.success) {
      const select = document.getElementById("userFilter");
      select.innerHTML = '<option value="">Tous</option>';

      result.data.forEach((user) => {
        const option = document.createElement("option");
        option.value = user.userName;
        option.textContent = user.fullName || user.userName;
        select.appendChild(option);
      });
    }
  } catch (error) {
    console.error("Error loading users:", error);
  }
}

// Apply filters to the table
function applyFilters() {
  const statusFilter = document.getElementById("statusFilter").value;
  const userFilter = document.getElementById("userFilter").value;
  const searchTerm = document.getElementById("searchInput").value.toLowerCase();
  const rows = document.querySelectorAll("#tasksTable tbody tr");

  let visibleCount = 0;

  rows.forEach((row) => {
    // Skip empty state row
    if (row.querySelector(".empty-state")) {
      return;
    }

    const cells = row.cells;
    const title = cells[1] ? cells[1].textContent.toLowerCase() : "";
    const description = cells[2] ? cells[2].textContent.toLowerCase() : "";
    const ticket = cells[3] ? cells[3].textContent.toLowerCase() : "";
    const client = cells[4] ? cells[4].textContent.toLowerCase() : "";
    const assignedUser = cells[5] ? cells[5].textContent : "";
    const statusBadge = row.querySelector(".status-badge");
    const currentStatus = statusBadge
      ? statusBadge.className.split(" ")[1].replace("status-", "")
      : "";

    // Check filters
    const matchesStatus =
      !statusFilter || currentStatus === statusFilter.toLowerCase();
    const matchesUser = !userFilter || assignedUser.includes(userFilter);
    const matchesSearch =
      !searchTerm ||
      title.includes(searchTerm) ||
      description.includes(searchTerm) ||
      ticket.includes(searchTerm) ||
      client.includes(searchTerm) ||
      assignedUser.toLowerCase().includes(searchTerm);

    const shouldShow = matchesStatus && matchesUser && matchesSearch;
    row.style.display = shouldShow ? "" : "none";

    if (shouldShow) {
      visibleCount++;
    }
  });

  updateEmptyState(visibleCount);
}

// Clear all filters
function clearFilters() {
  document.getElementById("searchInput").value = "";
  document.getElementById("statusFilter").value = "";
  document.getElementById("userFilter").value = "";
  applyFilters();
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
                <h3>Aucune tâche trouvée</h3>
                <p>Aucune tâche n'a été créée pour le moment.</p>
            `;
      emptyRow.style.display = "";
    } else {
      emptyRow.style.display = "none";
    }
  }
}

// Toggle select all checkboxes
function toggleSelectAll() {
  const selectAll = document.getElementById("selectAll");
  const checkboxes = document.querySelectorAll(".task-checkbox");

  checkboxes.forEach((checkbox) => {
    checkbox.checked = selectAll.checked;
  });

  updateDeleteButtonState();
}

// Update delete button state based on selection
function updateDeleteButtonState() {
  const checkboxes = document.querySelectorAll(".task-checkbox:checked");
  const deleteBtn = document.getElementById("deleteBtn");

  if (deleteBtn) {
    deleteBtn.disabled = checkboxes.length === 0;
  }
}

// Open create task modal
async function openCreateTaskModal() {
  document.getElementById("createTaskModalOverlay").style.display = "flex";

  // Load tickets and users
  await loadTicketsForAssignment();
  await loadUsersForAssignment();

  // Clear form
  document.getElementById("createTaskTitle").value = "";
  document.getElementById("createTaskDescription").value = "";
  document.getElementById("createTaskTicket").value = "";
  document.getElementById("createTaskAssignedUser").value = "";

  // Add event listener for ticket selection change
  const ticketSelect = document.getElementById("createTaskTicket");
  ticketSelect.addEventListener("change", async function () {
    const selectedTicketId = this.value;
    if (selectedTicketId) {
      await loadUsersForAssignment("create", selectedTicketId);
    } else {
      await loadUsersForAssignment("create");
    }
  });
}

// Close create task modal
function closeCreateTaskModal() {
  document.getElementById("createTaskModalOverlay").style.display = "none";
}

// Open edit task modal
async function openEditTaskModal(taskId) {
  document.getElementById("editTaskModalOverlay").style.display = "flex";

  // Load users for assignment
  await loadUsersForAssignment("edit");

  // Load task data
  await loadTaskForEdit(taskId);
}

// Close edit task modal
function closeEditTaskModal() {
  document.getElementById("editTaskModalOverlay").style.display = "none";
}

// Load tickets for assignment
async function loadTicketsForAssignment() {
  try {
    const response = await fetch("/DemandeClasses/GetTicketsForAssignment");
    const result = await response.json();

    if (result.success) {
      const select = document.getElementById("createTaskTicket");
      select.innerHTML =
        '<option value="">Sélectionner une demande...</option>';

      result.data.forEach((ticket) => {
        const option = document.createElement("option");
        option.value = ticket.id;
        option.textContent = `${ticket.title} (${ticket.clientName})`;
        select.appendChild(option);
      });
    }
  } catch (error) {
    console.error("Error loading tickets:", error);
  }
}

// Load users for assignment
async function loadUsersForAssignment(mode = "create", ticketId = null) {
  try {
    let url = "/DemandeClasses/GetUsersForAssignment";
    if (ticketId) {
      url += `?ticketId=${ticketId}`;
    }

    const response = await fetch(url);
    const result = await response.json();

    if (result.success) {
      const selectId =
        mode === "edit" ? "editTaskAssignedUser" : "createTaskAssignedUser";
      const select = document.getElementById(selectId);
      select.innerHTML =
        '<option value="">Sélectionner un utilisateur...</option>';

      result.data.forEach((user) => {
        const option = document.createElement("option");
        option.value = user.id;
        option.textContent = `${user.fullName || user.userName} (${
          user.teamName
        })`;
        select.appendChild(option);
      });
    }
  } catch (error) {
    console.error("Error loading users:", error);
  }
}

// Create new task
async function createTask() {
  const title = document.getElementById("createTaskTitle").value.trim();
  const description = document
    .getElementById("createTaskDescription")
    .value.trim();
  const ticketId = document.getElementById("createTaskTicket").value;
  const assignedUserId = document.getElementById(
    "createTaskAssignedUser"
  ).value;

  if (!title || !description || !ticketId || !assignedUserId) {
    alert("Veuillez remplir tous les champs obligatoires");
    return;
  }

  try {
    const response = await fetch("/DemandeClasses/CreateTask", {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify({
        title: title,
        description: description,
        ticketId: parseInt(ticketId),
        assignedUserId: parseInt(assignedUserId),
      }),
    });

    const result = await response.json();

    if (result.success) {
      showNotification("Tâche créée avec succès!", "success");
      closeCreateTaskModal();
      refreshTasks();
    } else {
      showNotification(
        "Erreur lors de la création de la tâche: " + result.message,
        "error"
      );
    }
  } catch (error) {
    console.error("Error creating task:", error);
    showNotification("Erreur lors de la création de la tâche", "error");
  }
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

// Show notification to user
function showNotification(message, type) {
  // Remove existing notifications
  const existingNotifications = document.querySelectorAll(".notification");
  existingNotifications.forEach((notification) => notification.remove());

  // Create new notification element
  const notification = document.createElement("div");
  notification.className = `notification ${type}`;
  notification.textContent = message;
  notification.style.cssText = `
        position: fixed;
        top: 20px;
        right: 20px;
        padding: 15px 20px;
        border-radius: 5px;
        color: white;
        font-weight: bold;
        z-index: 1000;
        box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
        ${
          type === "success"
            ? "background-color: #28a745;"
            : "background-color: #dc3545;"
        }
    `;

  document.body.appendChild(notification);

  // Auto-remove notification after 5 seconds
  setTimeout(() => {
    if (notification.parentNode) {
      notification.remove();
    }
  }, 5000);
}

// Refresh tasks data
async function refreshTasks() {
  try {
    window.location.reload();
  } catch (error) {
    console.error("Error refreshing tasks:", error);
    showNotification("Erreur lors de l'actualisation", "error");
  }
}

// Load task data for editing
async function loadTaskForEdit(taskId) {
  try {
    const response = await fetch(`/DemandeClasses/GetAllTasks`);
    const result = await response.json();

    if (result.success) {
      const task = result.data.find((t) => t.id === taskId);
      if (task) {
        document.getElementById("editTaskId").value = task.id;
        document.getElementById("editTaskTitle").value = task.title;
        document.getElementById("editTaskDescription").value = task.description;
        document.getElementById("editTaskStatus").value = task.status;
        document.getElementById("editTaskAssignedUser").value =
          task.assignedUserId;
      }
    }
  } catch (error) {
    console.error("Error loading task:", error);
  }
}

// Update task
async function updateTask() {
  const id = document.getElementById("editTaskId").value;
  const title = document.getElementById("editTaskTitle").value.trim();
  const description = document
    .getElementById("editTaskDescription")
    .value.trim();
  const status = document.getElementById("editTaskStatus").value;
  const assignedUserId = document.getElementById("editTaskAssignedUser").value;

  if (!title || !description || !assignedUserId) {
    alert("Veuillez remplir tous les champs obligatoires");
    return;
  }

  try {
    const response = await fetch("/DemandeClasses/UpdateTask", {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify({
        id: parseInt(id),
        title: title,
        description: description,
        status: status,
        assignedUserId: parseInt(assignedUserId),
      }),
    });

    const result = await response.json();

    if (result.success) {
      showNotification("Tâche mise à jour avec succès!", "success");
      closeEditTaskModal();
      refreshTasks();
    } else {
      showNotification(
        "Erreur lors de la mise à jour: " + result.message,
        "error"
      );
    }
  } catch (error) {
    console.error("Error updating task:", error);
    showNotification("Erreur lors de la mise à jour", "error");
  }
}

// Delete single task
async function deleteTask(taskId) {
  if (!confirm("Êtes-vous sûr de vouloir supprimer cette tâche ?")) {
    return;
  }

  try {
    const response = await fetch(`/DemandeClasses/DeleteTask/${taskId}`, {
      method: "DELETE",
    });

    const result = await response.json();

    if (result.success) {
      showNotification("Tâche supprimée avec succès!", "success");
      refreshTasks();
    } else {
      showNotification(
        "Erreur lors de la suppression: " + result.message,
        "error"
      );
    }
  } catch (error) {
    console.error("Error deleting task:", error);
    showNotification("Erreur lors de la suppression", "error");
  }
}

// Delete selected tasks
async function deleteSelectedTasks() {
  const checkboxes = document.querySelectorAll(".task-checkbox:checked");

  if (checkboxes.length === 0) {
    alert("Veuillez sélectionner au moins une tâche à supprimer");
    return;
  }

  if (
    !confirm(
      `Êtes-vous sûr de vouloir supprimer ${checkboxes.length} tâche(s) ?`
    )
  ) {
    return;
  }

  const taskIds = Array.from(checkboxes).map((cb) => parseInt(cb.value));

  try {
    const promises = taskIds.map((id) =>
      fetch(`/DemandeClasses/DeleteTask/${id}`, { method: "DELETE" })
    );

    const responses = await Promise.all(promises);
    const results = await Promise.all(responses.map((r) => r.json()));

    const successCount = results.filter((r) => r.success).length;
    const errorCount = results.length - successCount;

    if (successCount > 0) {
      showNotification(
        `${successCount} tâche(s) supprimée(s) avec succès!`,
        "success"
      );
    }

    if (errorCount > 0) {
      showNotification(
        `Erreur lors de la suppression de ${errorCount} tâche(s)`,
        "error"
      );
    }

    refreshTasks();
  } catch (error) {
    console.error("Error deleting tasks:", error);
    showNotification("Erreur lors de la suppression", "error");
  }
}

// Export functions for global access
window.openCreateTaskModal = openCreateTaskModal;
window.closeCreateTaskModal = closeCreateTaskModal;
window.openEditTaskModal = openEditTaskModal;
window.closeEditTaskModal = closeEditTaskModal;
window.createTask = createTask;
window.updateTask = updateTask;
window.deleteTask = deleteTask;
window.deleteSelectedTasks = deleteSelectedTasks;
window.clearFilters = clearFilters;
window.toggleSelectAll = toggleSelectAll;
window.refreshTasks = refreshTasks;
