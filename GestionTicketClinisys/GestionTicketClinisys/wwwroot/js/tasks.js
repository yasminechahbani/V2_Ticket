// tasks.js - For your Kanban interface
document.addEventListener('DOMContentLoaded', () => {
    // Load tasks for a ticket
    async function loadTasks(ticketId) {
        const response = await fetch(`/api/TaskItems/ByTicket/${ticketId}`);
        const tasks = await response.json();
        renderTasks(tasks);
    }

    // Render tasks to Kanban columns
    function renderTasks(tasks) {
        const columns = {
            'open': document.querySelector('.column-open'),
            'in-progress': document.querySelector('.column-in-progress'),
            'resolved': document.querySelector('.column-resolved')
        };

        // Clear existing tasks
        Object.values(columns).forEach(col => {
            col.querySelectorAll('.task-card').forEach(card => card.remove());
        });

        // Add tasks to appropriate columns
        tasks.forEach(task => {
            const column = columns[task.status.toLowerCase().replace(' ', '-')];
            if (column) {
                column.appendChild(createTaskCard(task));
            }
        });
    }

    // Create task card HTML
    function createTaskCard(task) {
        const card = document.createElement('div');
        card.className = 'task-card';
        card.dataset.taskId = task.id;
        card.draggable = true;

        card.innerHTML = `
            <div class="task-header">
                <span class="priority-badge priority-${task.priority}"></span>
                <h4>${task.title}</h4>
            </div>
            <p>${task.description.substring(0, 50)}...</p>
            <div class="task-footer">
                <span class="due-date">${new Date(task.dueDate).toLocaleDateString()}</span>
                <span class="assignee">${task.assignedUser.userName}</span>
            </div>
        `;

        // Add drag events
        card.addEventListener('dragstart', handleDragStart);
        return card;
    }

    // Drag-and-drop implementation
    function handleDragStart(e) {
        e.dataTransfer.setData('text/plain', e.target.dataset.taskId);
    }

    document.querySelectorAll('.kanban-column').forEach(column => {
        column.addEventListener('dragover', e => e.preventDefault());
        column.addEventListener('drop', async (e) => {
            e.preventDefault();
            const taskId = e.dataTransfer.getData('text/plain');
            const newStatus = e.target.closest('.kanban-column').dataset.status;

            try {
                const response = await fetch(`/api/TaskItems/${taskId}/Status`, {
                    method: 'PUT',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify(newStatus)
                });

                if (response.ok) {
                    loadTasks(currentTicketId); // Refresh view
                }
            } catch (error) {
                console.error('Error updating task status:', error);
            }
        });
    });

    // Initialize
    const currentTicketId = document.getElementById('ticket-id').value;
    loadTasks(currentTicketId);
});